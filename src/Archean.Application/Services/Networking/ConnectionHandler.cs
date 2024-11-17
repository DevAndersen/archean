using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Services.Networking;
using System.Buffers.Binary;
using System.Net.Sockets;

namespace Archean.Application.Services.Networking;

public class ConnectionHandler : IConnectionHandler
{
    private readonly IClientPacketReader clientPacketReader;
    private readonly IConnectionRepository connectionRepository;
    private readonly IPacketDataReader packetDataReader;
    private readonly IServerPacketWriter serverPacketWriter;
    private readonly ILogger<ConnectionHandler> logger;

    public ConnectionHandler(IClientPacketReader clientPacketReader, IConnectionRepository connectionRepository, IPacketDataReader packetDataReader, IServerPacketWriter serverPacketWriter, ILogger<ConnectionHandler> logger)
    {
        this.clientPacketReader = clientPacketReader;
        this.connectionRepository = connectionRepository;
        this.packetDataReader = packetDataReader;
        this.serverPacketWriter = serverPacketWriter;
        this.logger = logger;
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
                "todo"); // Todo

            await connection.SendAsync(serverPacketWriter.WriteDisconnectPlayerPacket(new ServerDisconnectPlayerPacket
            {
                Message = $"Invalid client identification packet ID {(byte)packetId}"
            }));

            connection.Disconnect();
            return;
        }

        ClientIdentificationPacket clientIdentificationPacket = clientPacketReader.ReadIdentificationPacket(buffer);

        // Validate client protocol ID.
        if (clientIdentificationPacket.ProtocolVersion != Constants.Networking.ProtocolVersion)
        {
            logger.LogError("Connection {connectionId} specified unexpected protocol version {protocolVersion}",
                "todo", // Todo
                clientIdentificationPacket.ProtocolVersion);

            await connection.SendAsync(serverPacketWriter.WriteDisconnectPlayerPacket(new ServerDisconnectPlayerPacket
            {
                Message = $"Invalid client protocol version {clientIdentificationPacket.ProtocolVersion}"
            }));

            connection.Disconnect();
            return;
        }

        connectionRepository.TryAddConnection(connection);

        await SendJoinServerTestAsync(connection, clientIdentificationPacket);
        new Thread(async () => await ReceiveFromClientAsync(connection, cancellationToken)).Start();
    }

    private async Task ReceiveFromClientAsync(IConnection connection, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && connection.IsConnected)
            {
                ReadOnlyMemory<byte> buffer = await connection.ReadAsync(cancellationToken);

                while (buffer.Length > 0)
                {
                    ClientPacketId packetId = (ClientPacketId)buffer.Span[0];

                    // Slice the packet ID byte off.
                    buffer = buffer.Slice(1);

                    switch (packetId)
                    {
                        // Block update packet.
                        case ClientPacketId.SetBlock:
                            buffer = buffer.Slice(ClientSetBlockPacket.PacketSize);
                            break;

                        // Pose packet.
                        case ClientPacketId.PositionAndOrientation:
                            buffer = buffer.Slice(ClientPositionAndOrientationPacket.PacketSize);
                            break;

                        // Message packet.
                        case ClientPacketId.Message:
                            buffer = buffer.Slice(ClientMessagePacket.PacketSize);
                            break;

                        // Unknown packet type.
                        default:
                            logger.LogWarning("Unexpected packet {packetId} receives from {connectionIdentity}, flushing buffer",
                                packetId,
                                "Todo"); // Todo

                            buffer = buffer.Slice(buffer.Length);
                            break;
                    }
                }
            }
        }
        catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionReset)
        {
            logger.LogInformation("{connectionIdentity} disconnected",
                "Todo"); // Todo
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected {exceptionType} thrown while receiving data from connection {connectionIdentity}",
                e.GetType().FullName,
                "Todo"); // Todo

            throw;
        }
        finally
        {
            connectionRepository.Remove(connection);
            connection.Disconnect();
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

        await connection.SendAsync(serverPacketWriter.WritePacket(new ServerLevelInitializePacket()));
        await SendLevelTestAsync(connection);

        await connection.SendAsync(serverPacketWriter.WritePacket(new ServerLevelFinalizePacket
        {
            XSize = 128,
            YSize = 64,
            ZSize = 128,
        }));

        await connection.SendAsync(serverPacketWriter.WritePacket(new ServerSpawnPlayerPacket
        {
            PlayerId = Constants.Networking.PlayerSelfId,
            PlayerName = clientIdentificationPacket.Username,
            X = new FShort(4F),
            Y = new FShort(4F),
            Z = new FShort(4F),
            Pitch = 0,
            Yaw = 0
        }));
    }

    private async Task SendLevelTestAsync(IConnection connection)
    {
        int blockCount = 128 * 64 * 128;
        byte[] blocks = new byte[blockCount + sizeof(int)];

        BinaryPrimitives.WriteInt32BigEndian(blocks, blockCount);
        blocks[4] = (byte)Block.Bedrock;

        byte[] blockGZipBuffer = GZipHelper.Compress(blocks);

        byte[][] chunks = blockGZipBuffer.Chunk(Constants.Networking.ByteArrayLength).ToArray();
        for (int i = 0; i < chunks.Length; i++)
        {
            Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength];

            chunks[i].CopyTo(buffer);
            byte percent = (byte)((i + 1) / (float)chunks.Length * 100);

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
                ChunkLength = (short)chunks[i].Length,
                PercentageComplete = percent
            }));
        }
    }
}
