namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerRelativePositionAndOrientationPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + FByte.Size // X
        + FByte.Size // Y
        + FByte.Size // Z
        + sizeof(byte) // Yaw
        + sizeof(byte); // Pitch

    public ServerPacketId PacketId => ServerPacketId.RelativePositionAndOrientation;

    public required sbyte PlayerId { get; init; }

    public required FByte X { get; init; }

    public required FByte Y { get; init; }

    public required FByte Z { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
