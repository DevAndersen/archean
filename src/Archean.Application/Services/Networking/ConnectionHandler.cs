using Archean.Application.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace Archean.Application.Services.Networking;

public class ConnectionHandler : IConnectionHandler
{
    private readonly IClientPacketReader _clientPacketReader;
    private readonly IPlayerRegistry _playerRegistry;
    private readonly IPacketDataReader _packetDataReader;
    private readonly ILogger<ConnectionHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IWorldRegistry _worldRegistry;
    private readonly IGlobalEventBus _globalEventBus;
    private readonly ServerSettings _serverSettings;

    public ConnectionHandler(
        IClientPacketReader clientPacketReader,
        IPlayerRegistry playerRegistry,
        IPacketDataReader packetDataReader,
        ILogger<ConnectionHandler> logger,
        IServiceScopeFactory serviceScopeFactory,
        IWorldRegistry worldRegistry,
        IGlobalEventBus globalEventBus,
        IOptions<ServerSettings> serverSettingsOptions)
    {
        _clientPacketReader = clientPacketReader;
        _playerRegistry = playerRegistry;
        _packetDataReader = packetDataReader;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _worldRegistry = worldRegistry;
        _globalEventBus = globalEventBus;
        _serverSettings = serverSettingsOptions.Value;
    }

    public async Task HandleNewConnectionAsync(IConnection connection, CancellationToken cancellationToken)
    {
        ReadOnlyMemory<byte> buffer = await connection.ReadAsync(cancellationToken);
        ClientPacketId packetId = (ClientPacketId)_packetDataReader.ReadByte(buffer, out buffer);

        // Ensure that the first packet ID corresponds to that of an identification packet.
        if (packetId != ClientPacketId.Identification)
        {
            _logger.LogError("Unexpected first packet ID {packetId} from connection {connectionId}",
                packetId,
                connection.Id);

            await connection.SendAsync(new ServerDisconnectPlayerPacket
            {
                Message = $"Invalid client identification packet ID {(byte)packetId}"
            });

            await connection.DisconnectAsync();
            return;
        }

        ClientIdentificationPacket clientIdentificationPacket = _clientPacketReader.ReadIdentificationPacket(buffer);

        // Validate client protocol ID.
        if (clientIdentificationPacket.ProtocolVersion != Constants.Networking.ProtocolVersion)
        {
            _logger.LogError("Connection {connectionId} specified unexpected protocol version {protocolVersion}",
                connection.Id,
                clientIdentificationPacket.ProtocolVersion);

            await connection.SendAsync(new ServerDisconnectPlayerPacket
            {
                Message = $"Invalid client protocol version {clientIdentificationPacket.ProtocolVersion}"
            });

            await connection.DisconnectAsync();
            return;
        }

        IPlayer player = new Player(
            connection,
            clientIdentificationPacket.Username,
            _globalEventBus);

        if (_playerRegistry.TryAdd(player))
        {
            _logger.LogInformation("Player {username} assigned ID {playerId}",
                player.Username,
                player.Id);

            await SendJoinServerTestAsync(player);
            new Thread(async () => await ReceiveFromClientAsync(player, cancellationToken)).Start();
        }
        else
        {
            _logger.LogInformation("Unable to register player {username}, server is full",
                player.Username);

            await connection.DisconnectAsync(_serverSettings.ServerFullMessage);
        }
    }

    private async Task ReceiveFromClientAsync(IPlayer player, CancellationToken cancellationToken)
    {
        IConnection connection = player.Connection;

        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        // Set the player of the current scope.
        IPlayerService playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>();
        playerService.SetPlayer(player);

        // Register event subscriptions.
        IClientEventHandler eventHandler = scope.ServiceProvider.GetRequiredService<IClientEventHandler>();
        eventHandler.RegisterEventSubscriptions();

        IClientPacketHandler packetHandler = scope.ServiceProvider.GetRequiredService<IClientPacketHandler>();

        try
        {
            while (!cancellationToken.IsCancellationRequested && connection.IsConnected)
            {
                ReadOnlyMemory<byte> buffer = await connection.ReadAsync(cancellationToken);

                while (buffer.Length > 0)
                {
                    ClientPacketId packetId = (ClientPacketId)buffer.Span[0];

                    // Slice the packet ID byte off.
                    buffer = buffer[1..];

                    switch (packetId)
                    {
                        // Block update packet.
                        case ClientPacketId.SetBlock:
                            ClientSetBlockPacket setBlockPacket = _clientPacketReader.ReadSetBlockPacket(buffer[..ClientSetBlockPacket.PacketSize]);
                            buffer = buffer[ClientSetBlockPacket.PacketSize..];
                            await packetHandler.HandleSetBlockPacketAsync(setBlockPacket);
                            break;

                        // Pose packet.
                        case ClientPacketId.PositionAndOrientation:
                            ClientPositionAndOrientationPacket positionAndOrientationPacket = _clientPacketReader.ReadPositionAndOrientationPacket(buffer[..ClientPositionAndOrientationPacket.PacketSize]);
                            buffer = buffer[ClientPositionAndOrientationPacket.PacketSize..];
                            await packetHandler.HandlePositionAndOrientationPacketAsync(positionAndOrientationPacket);
                            break;

                        // Message packet.
                        case ClientPacketId.Message:
                            ClientMessagePacket messagePacket = _clientPacketReader.ReadMessagePacket(buffer[..ClientMessagePacket.PacketSize]);
                            buffer = buffer[ClientMessagePacket.PacketSize..];
                            await packetHandler.HandleMessagePacketAsync(messagePacket);
                            break;

                        // Unknown packet type.
                        default:
                            _logger.LogWarning("Unexpected packet {packetId} receives from {connectionId}, flushing buffer",
                                packetId,
                                connection.Id);

                            buffer = buffer[buffer.Length..];
                            break;
                    }
                }
            }
        }
        catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionReset)
        {
            _logger.LogInformation("Connection {connectionId} disconnected",
                connection.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected {exceptionType} thrown while receiving data from connection {connectionId}",
                e.GetType().FullName,
                connection.Id);
        }
        finally
        {
            _playerRegistry.Remove(connection);

            await _globalEventBus.InvokeEventAsync(new PlayerDisconnectEvent
            {
                Player = player
            });

            await connection.DisconnectAsync();
        }
    }

    private async Task SendJoinServerTestAsync(IPlayer player)
    {
        await player.Connection.SendAsync(new ServerIdentificationPacket
        {
            PlayerType = PlayerType.Normal,
            ProtocolVersion = Constants.Networking.ProtocolVersion,
            ServerMotd = _serverSettings.Motd,
            ServerName = _serverSettings.Name,
        });

        await _worldRegistry.GetDefaultWorld().JoinAsync(player);
    }
}
