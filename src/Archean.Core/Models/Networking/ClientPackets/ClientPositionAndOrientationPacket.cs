namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientPositionAndOrientationPacket : IClientPacket
{
    public required FShort X { get; init; }

    public required FShort Y { get; init; }

    public required FShort Z { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
