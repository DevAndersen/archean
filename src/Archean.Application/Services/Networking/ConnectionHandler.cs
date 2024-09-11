using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Services.Networking;

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

    public async Task HandleNewConnectionAsync(IConnection connection)
    {
        ReadOnlyMemory<byte> buffer = await connection.ReadAsync();
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
    }
}
