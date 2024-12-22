using Archean.Core.Models.Networking;

namespace Archean.Core.Models.Events;

public class SetBlockEvent : Event
{
    public IPlayer? Player { get; init; }

    public required short X { get; init; }

    public required short Y { get; init; }

    public required short Z { get; init; }

    public required BlockChangeMode Mode { get; init; }

    public required Block Block { get; init; }

    public required Block HeldBlock { get; init; }
}
