using Archean.Application.Models;
using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Networking;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace Archean.Application.Services.Networking;

public class ConnectionHandler : IConnectionHandler
{
    private readonly IClientPacketReader clientPacketReader;
    private readonly IPlayerRegistry playerRegistry;
    private readonly IPacketDataReader packetDataReader;
    private readonly IServerPacketWriter serverPacketWriter;
    private readonly ILogger<ConnectionHandler> logger;
    private readonly IServiceProvider provider;

    public ConnectionHandler(
        IClientPacketReader clientPacketReader,
        IPlayerRegistry playerRegistry,
        IPacketDataReader packetDataReader,
        IServerPacketWriter serverPacketWriter,
        ILogger<ConnectionHandler> logger,
        IServiceProvider provider)
    {
        this.clientPacketReader = clientPacketReader;
        this.playerRegistry = playerRegistry;
        this.packetDataReader = packetDataReader;
        this.serverPacketWriter = serverPacketWriter;
        this.logger = logger;
        this.provider = provider;
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

            await connection.SendAsync(serverPacketWriter.WriteDisconnectPlayerPacket(new ServerDisconnectPlayerPacket
            {
                Message = $"Invalid client identification packet ID {(byte)packetId}"
            }));

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

            await connection.SendAsync(serverPacketWriter.WriteDisconnectPlayerPacket(new ServerDisconnectPlayerPacket
            {
                Message = $"Invalid client protocol version {clientIdentificationPacket.ProtocolVersion}"
            }));

            await connection.DisconnectAsync();
            return;
        }

        IPlayer player = new Player(connection, clientIdentificationPacket.Username);

        if (playerRegistry.TryAdd(player))
        {
            logger.LogInformation("Player {username} assigned ID {playerId}",
                player.Username,
                player.Id);

            await SendJoinServerTestAsync(connection, clientIdentificationPacket);
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
            // Todo: Player disconnect event.
            await connection.DisconnectAsync();
        }
    }

    private async Task SendJoinServerTestAsync(IConnection connection, ClientIdentificationPacket clientIdentificationPacket)
    {
        await connection.SendAsync(serverPacketWriter.WritePacket(new ServerIdentificationPacket
        {
            PlayerType = PlayerType.Normal,
            ProtocolVersion = Constants.Networking.ProtocolVersion,
            ServerMotd = "Server MOTD",
            ServerName = "Server name",
        }));

        short width = 16;
        short height = 16;
        short depth = 16;

        BlockMap blockMap = new BlockMap(width, height, depth);

        for (int x = 0; x < blockMap.Width; x++)
        {
            for (int z = 0; z < blockMap.Depth; z++)
            {
                for (int y = 0; y < 7; y++)
                {
                    blockMap[x, y, z] = Block.Dirt;
                }

                blockMap[x, 7, z] = Block.Grass;
            }
        }

        await connection.SendAsync(serverPacketWriter.WritePacket(new ServerLevelInitializePacket()));
        await SendLevelTestAsync(blockMap, connection);

        await connection.SendAsync(serverPacketWriter.WritePacket(new ServerLevelFinalizePacket
        {
            XSize = blockMap.Width,
            YSize = blockMap.Height,
            ZSize = blockMap.Depth,
        }));

        await connection.SendAsync(serverPacketWriter.WritePacket(new ServerSpawnPlayerPacket
        {
            PlayerId = Constants.Networking.PlayerSelfId,
            PlayerName = clientIdentificationPacket.Username,
            X = new FShort(4F),
            Y = new FShort(height + 3),
            Z = new FShort(4F),
            Pitch = 0,
            Yaw = 0
        }));
    }

    private async Task SendLevelTestAsync(BlockMap blockMap, IConnection connection)
    {
        byte[] blockGZipBuffer = GZipHelper.Compress(blockMap.AsMemory().Span);
        int chunkCount = (blockGZipBuffer.Length / Constants.Networking.ByteArrayLength) + 1;

        for (int i = 0; i < chunkCount; i++)
        {
            int start = i * Constants.Networking.ByteArrayLength;
            int end = Math.Min(blockGZipBuffer.Length - start, Constants.Networking.ByteArrayLength);
            int length = end - start;
            byte percent = (byte)((i + 1) / (float)chunkCount * 100);

            Memory<byte> chunk = blockGZipBuffer
                .AsMemory()
                .Slice(start, end);

            Memory<byte> buffer;
            if (length < Constants.Networking.ByteArrayLength)
            {
                buffer = new byte[Constants.Networking.ByteArrayLength];
                chunk.CopyTo(buffer);
            }
            else
            {
                buffer = chunk;
            }

            await connection.SendAsync(serverPacketWriter.WritePacket(new ServerIdentificationPacket
            {
                PlayerType = PlayerType.Op,
                ProtocolVersion = Constants.Networking.ProtocolVersion,
                ServerMotd = $"You are {percent}% done",
                ServerName = "Server name",
            }));

            await connection.SendAsync(serverPacketWriter.WritePacket(new ServerLevelDataChunkPacket
            {
                ChunkData = buffer.ToArray(),
                ChunkLength = (short)length,
                PercentageComplete = percent
            }));
        }
    }
}
