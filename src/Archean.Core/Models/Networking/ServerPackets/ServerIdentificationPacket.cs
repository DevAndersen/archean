namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerIdentificationPacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.Identification;

    public required byte ProtocolVersion { get; init; }

    public required string ServerName { get; init; }

    public required string ServerMotd { get; init; }

    public required PlayerType PlayerType { get; init; }
}
