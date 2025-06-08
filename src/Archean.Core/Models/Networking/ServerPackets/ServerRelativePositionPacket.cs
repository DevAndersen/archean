namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerRelativePositionPacket : IServerPacket
{
    public required sbyte PlayerId { get; init; }

    public required FByte X { get; init; }

    public required FByte Y { get; init; }

    public required FByte Z { get; init; }
}
