namespace Archean.Core.Models.Events;

/// <summary>
/// Represents a chat message event.
/// </summary>
public class MessageEvent : Event
{
    public required IPlayer? PlayerSender { get; init; }

    public required string Message { get; init; }
}
