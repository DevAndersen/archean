namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerRelativeOrientationPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + sizeof(byte) // Yaw
        + sizeof(byte); // Pitch

    public ServerPacketId PacketId => ServerPacketId.RelativeOrientation;

    public required sbyte PlayerId { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
