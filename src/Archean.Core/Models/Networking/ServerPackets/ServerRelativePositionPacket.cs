namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerRelativePositionPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + FByte.Size // X
        + FByte.Size // Y
        + FByte.Size; // Z

    public ServerPacketId PacketId => ServerPacketId.RelativePosition;

    public required sbyte PlayerId { get; init; }

    public required FByte X { get; init; }

    public required FByte Y { get; init; }

    public required FByte Z { get; init; }
}
