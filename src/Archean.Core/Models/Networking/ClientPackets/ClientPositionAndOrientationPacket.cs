namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientPositionAndOrientationPacket : IClientPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(byte) // Player ID, unused
        + FShort.Size // X
        + FShort.Size // Y
        + FShort.Size // Z
        + sizeof(byte) // YaW
        + sizeof(byte); // Pitch

    public ClientPacketId PacketId => ClientPacketId.PositionAndOrientation;

    public required FShort X { get; init; }

    public required FShort Y { get; init; }

    public required FShort Z { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
