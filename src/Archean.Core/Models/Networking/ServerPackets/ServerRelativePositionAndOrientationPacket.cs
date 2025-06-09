namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerRelativePositionAndOrientationPacket : IServerPacket
{
    public required sbyte PlayerId { get; init; }

    /// <remarks>
    /// 8-bit fixed point decimal.
    /// </remarks>
    public required float X { get; init; }

    /// <remarks>
    /// 8-bit fixed point decimal.
    /// </remarks>
    public required float Y { get; init; }

    /// <remarks>
    /// 8-bit fixed point decimal.
    /// </remarks>
    public required float Z { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
