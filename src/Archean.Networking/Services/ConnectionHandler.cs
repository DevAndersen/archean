using Archean.Core;
using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Archean.Networking.Helpers;
using Archean.Networking.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;

namespace Archean.Networking.Services;

public class ConnectionHandler : IConnectionHandler
{
    private readonly IPlayerRegistry _playerRegistry;
    private readonly ILogger<ConnectionHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IWorldRegistry _worldRegistry;
    private readonly IGlobalEventBus _globalEventBus;
    private readonly ServerSettings _serverSettings;

    public ConnectionHandler(
        IPlayerRegistry playerRegistry,
        ILogger<ConnectionHandler> logger,
        IServiceScopeFactory serviceScopeFactory,
        IWorldRegistry worldRegistry,
        IGlobalEventBus globalEventBus,
        IOptions<ServerSettings> serverSettingsOptions)
    {
        _playerRegistry = playerRegistry;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _worldRegistry = worldRegistry;
        _globalEventBus = globalEventBus;
        _serverSettings = serverSettingsOptions.Value;
    }

    public async Task HandleNewConnectionAsync(IConnection connection, CancellationToken cancellationToken)
    {
        ReadOnlyMemory<byte> buffer = await connection.ReadAsync(cancellationToken);
        IEnumerable<IClientPacket> initialConnectionPackets = ClientPacketDeserializer.ReadPackets(buffer, cancellationToken);

        if (initialConnectionPackets.ToArray() is not [ClientIdentificationPacket clientIdentificationPacket])
        {
            _logger.LogError("Unexpected initial packets received from connection {connectionId}",
                connection.Id);

            await connection.DisconnectAsync();
            return;
        }

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

                foreach (IClientPacket packet in ClientPacketDeserializer.ReadPackets(buffer, cancellationToken))
                {
                    switch (packet)
                    {
                        // Client identification packet.
                        case ClientIdentificationPacket clientIdentificationPacket:
                            // Todo: Handle unexpected client identification packet.
                            break;

                        // Block update packet.
                        case ClientSetBlockPacket setBlockPacket:
                            await packetHandler.HandleSetBlockPacketAsync(setBlockPacket);
                            break;

                        // Pose packet.
                        case ClientPositionAndOrientationPacket positionAndOrientationPacket:
                            await packetHandler.HandlePositionAndOrientationPacketAsync(positionAndOrientationPacket);
                            break;

                        // Message packet.
                        case ClientMessagePacket messagePacket:
                            await packetHandler.HandleMessagePacketAsync(messagePacket);
                            break;

                        // Unknown packet type.
                        default:
                            // Todo: Handle unknown packet.
                            _logger.LogWarning("Unexpected packet {packetType} receives from {connectionId}, ", // Todo: Finish error message
                                packet.GetType().FullName,
                                connection.Id);
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
