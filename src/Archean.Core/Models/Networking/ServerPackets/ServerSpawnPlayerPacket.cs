namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerSpawnPlayerPacket : IServerPacket
{
    public required sbyte PlayerId { get; init; }

    public required string PlayerName { get; init; }

    public required FShort X { get; init; }

    public required FShort Y { get; init; }

    public required FShort Z { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
