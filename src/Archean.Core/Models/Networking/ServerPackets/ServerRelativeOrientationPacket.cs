namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerRelativeOrientationPacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.RelativeOrientation;

    public required sbyte PlayerId { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
