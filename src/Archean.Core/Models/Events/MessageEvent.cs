namespace Archean.Core.Models.Events;

public class MessageEvent : Event
{
    public required string Message { get; init; }
}
