namespace Archean.Core.Models.Events;

public class PlayerDisconnectEvent : Event
{
    public required IPlayer Player { get; init; }
}
