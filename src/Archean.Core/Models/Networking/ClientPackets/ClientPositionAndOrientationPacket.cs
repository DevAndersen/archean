namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientPositionAndOrientationPacket : IClientPacket
{
    /// <remarks>
    /// 16-bit fixed point decimal.
    /// </remarks>
    public required float X { get; init; }

    /// <remarks>
    /// 16-bit fixed point decimal.
    /// </remarks>
    public required float Y { get; init; }

    /// <remarks>
    /// 16-bit fixed point decimal.
    /// </remarks>
    public required float Z { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
