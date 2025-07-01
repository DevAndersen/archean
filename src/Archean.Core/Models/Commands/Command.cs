namespace Archean.Core.Models.Commands;

public abstract class Command
{
    public IPlayer? InvokingPlayer { get; set; }

    public abstract Task InvokeAsync();
}
