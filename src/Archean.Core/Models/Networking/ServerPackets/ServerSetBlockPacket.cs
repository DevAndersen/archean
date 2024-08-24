namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerSetBlockPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(short) // X
        + sizeof(short) // Y
        + sizeof(short) // Z
        + sizeof(Block); // Block

    public ServerPacketId PacketId => ServerPacketId.SetBlock;

    public required short X { get; init; }

    public required short Y { get; init; }

    public required short Z { get; init; }

    public required Block BlockType { get; init; }
}
