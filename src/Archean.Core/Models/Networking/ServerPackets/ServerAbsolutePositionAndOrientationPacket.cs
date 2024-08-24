namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerAbsolutePositionAndOrientationPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + FShort.Size // X
        + FShort.Size // Y
        + FShort.Size // Z
        + sizeof(byte) // Yaw
        + sizeof(byte); // Pitch

    public ServerPacketId PacketId => ServerPacketId.AbsolutePositionAndOrientation;

    public required sbyte PlayerId { get; init; }

    public required FShort X { get; init; }

    public required FShort Y { get; init; }

    public required FShort Z { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
