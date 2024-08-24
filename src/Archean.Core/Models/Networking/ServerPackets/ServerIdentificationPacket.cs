namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerIdentificationPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(byte) // ProtocolVersion
        + Constants.Networking.StringLength // ServerName
        + Constants.Networking.StringLength // ServerMotd
        + sizeof(PlayerType); // PlayerType

    public ServerPacketId PacketId => ServerPacketId.Identification;

    public required byte ProtocolVersion { get; init; }

    public required string ServerName { get; init; }

    public required string ServerMotd { get; init; }

    public required PlayerType PlayerType { get; init; }
}
