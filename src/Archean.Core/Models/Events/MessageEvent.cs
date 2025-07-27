namespace Archean.Core.Models.Events;

public class MessageEvent : Event
{
    public required IPlayer? PlayerSender { get; init; }

    public required string Message { get; init; }
}
