namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerUpdatePlayerTypePacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(PlayerType); // Player type

    public ServerPacketId PacketId => ServerPacketId.UpdatePlayerType;

    public required PlayerType PlayerType { get; init; }
}
