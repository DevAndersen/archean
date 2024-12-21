namespace Archean.Core.Models.Events;

public class PositionAndOrientationEvent : Event
{
    public required IPlayer Player { get; init; }

    public required float X { get; init; }

    public required float Y { get; init; }

    public required float Z { get; init; }

    public required byte Yaw { get; init; }

    public required byte Pitch { get; init; }
}
