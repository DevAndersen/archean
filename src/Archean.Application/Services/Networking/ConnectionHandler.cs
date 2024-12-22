using Archean.Application.Models;
using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;
using Archean.Core.Services.Worlds;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace Archean.Application.Services.Networking;

public class ConnectionHandler : IConnectionHandler
{
    private readonly IClientPacketReader clientPacketReader;
    private readonly IPlayerRegistry playerRegistry;
    private readonly IPacketDataReader packetDataReader;
    private readonly ILogger<ConnectionHandler> logger;
    private readonly IServiceProvider provider;
    private readonly IWorldRegistry worldRegistry;
    private readonly IGlobalEventBus globalEventBus;

    public ConnectionHandler(
        IClientPacketReader clientPacketReader,
        IPlayerRegistry playerRegistry,
        IPacketDataReader packetDataReader,
        ILogger<ConnectionHandler> logger,
        IServiceProvider provider,
        IWorldRegistry worldRegistry,
        IGlobalEventBus globalEventBus)
    {
        this.clientPacketReader = clientPacketReader;
        this.playerRegistry = playerRegistry;
        this.packetDataReader = packetDataReader;
        this.logger = logger;
        this.provider = provider;
        this.worldRegistry = worldRegistry;
        this.globalEventBus = globalEventBus;
    }

    public async Task HandleNewConnectionAsync(IConnection connection, CancellationToken cancellationToken)
    {
        ReadOnlyMemory<byte> buffer = await connection.ReadAsync(cancellationToken);
        ClientPacketId packetId = (ClientPacketId)packetDataReader.ReadByte(buffer, out buffer);

        // Ensure that the first packet ID corresponds to that of an identification packet.
        if (packetId != ClientPacketId.Identification)
        {
            logger.LogError("Unexpected first packet ID {packetId} from connection {connectionId}",
                packetId,
                connection.Id);

            await connection.SendAsync(new ServerDisconnectPlayerPacket
            {
                Message = $"Invalid client identification packet ID {(byte)packetId}"
            });

            await connection.DisconnectAsync();
            return;
        }

        ClientIdentificationPacket clientIdentificationPacket = clientPacketReader.ReadIdentificationPacket(buffer);

        // Validate client protocol ID.
        if (clientIdentificationPacket.ProtocolVersion != Constants.Networking.ProtocolVersion)
        {
            logger.LogError("Connection {connectionId} specified unexpected protocol version {protocolVersion}",
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
            globalEventBus);

        if (playerRegistry.TryAdd(player))
        {
            logger.LogInformation("Player {username} assigned ID {playerId}",
                player.Username,
                player.Id);

            await SendJoinServerTestAsync(player, clientIdentificationPacket);
            new Thread(async () => await ReceiveFromClientAsync(player, cancellationToken)).Start();
        }
        else
        {
            logger.LogError("Unable to register player {username}, server is full",
                player.Username);

            await connection.DisconnectAsync("The server is full");
        }
    }

    private async Task ReceiveFromClientAsync(IPlayer player, CancellationToken cancellationToken)
    {
        IConnection connection = player.Connection;

        using IServiceScope scope = provider.CreateScope();

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
                            ClientSetBlockPacket setBlockPacket = clientPacketReader.ReadSetBlockPacket(buffer[..ClientSetBlockPacket.PacketSize]);
                            buffer = buffer[ClientSetBlockPacket.PacketSize..];
                            await packetHandler.HandleSetBlockPacketAsync(setBlockPacket);
                            break;

                        // Pose packet.
                        case ClientPacketId.PositionAndOrientation:
                            ClientPositionAndOrientationPacket positionAndOrientationPacket = clientPacketReader.ReadPositionAndOrientationPacket(buffer[..ClientPositionAndOrientationPacket.PacketSize]);
                            buffer = buffer[ClientPositionAndOrientationPacket.PacketSize..];
                            await packetHandler.HandlePositionAndOrientationPacketAsync(positionAndOrientationPacket);
                            break;

                        // Message packet.
                        case ClientPacketId.Message:
                            ClientMessagePacket messagePacket = clientPacketReader.ReadMessagePacket(buffer[..ClientMessagePacket.PacketSize]);
                            buffer = buffer[ClientMessagePacket.PacketSize..];
                            await packetHandler.HandleMessagePacketAsync(messagePacket);
                            break;

                        // Unknown packet type.
                        default:
                            logger.LogWarning("Unexpected packet {packetId} receives from {connectionId}, flushing buffer",
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
            logger.LogInformation("Connection {connectionId} disconnected",
                connection.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected {exceptionType} thrown while receiving data from connection {connectionId}",
                e.GetType().FullName,
                connection.Id);
        }
        finally
        {
            playerRegistry.Remove(connection);

            await globalEventBus.InvokeEventAsync(new PlayerDisconnectEvent
            {
                Player = player
            });

            await connection.DisconnectAsync();
        }
    }

    private async Task SendJoinServerTestAsync(IPlayer player, ClientIdentificationPacket clientIdentificationPacket)
    {
        await player.Connection.SendAsync(new ServerIdentificationPacket
        {
            PlayerType = PlayerType.Normal,
            ProtocolVersion = Constants.Networking.ProtocolVersion,
            ServerMotd = "Server MOTD",
            ServerName = "Server name",
        });

        await worldRegistry.GetDefaultWorld().JoinAsync(player);
    }
}
