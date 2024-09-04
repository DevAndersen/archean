using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Services.Networking;
using Microsoft.Extensions.Logging;

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

        if (packetId != ClientPacketId.Identification)
        {
            logger.LogError("Unexpected first packet ID {packetId} from connection {connectionId}",
                packetId,
                "todo"); // todo
        }

        clientPacketReader.ReadIdentificationPacket(buffer);
        connectionRepository.TryAddConnection(connection);
    }
}
