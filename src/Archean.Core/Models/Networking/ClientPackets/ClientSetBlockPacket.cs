namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientSetBlockPacket : IClientPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(short) // X
        + sizeof(short) // Y
        + sizeof(short) // Z
        + sizeof(BlockChangeMode) // Mode
        + sizeof(Block); // Block

    public ClientPacketId PacketId => ClientPacketId.SetBlock;

    public required short X { get; init; }

    public required short Y { get; init; }

    public required short Z { get; init; }

    public required BlockChangeMode Mode { get; init; }

    public required Block BlockType { get; init; }
}
