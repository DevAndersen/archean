namespace Archean.Core.Models.Events;

public class MessageEvent : Event
{
    public IPlayer? PlayerSender { get; init; }

    public required string Message { get; init; }
}
