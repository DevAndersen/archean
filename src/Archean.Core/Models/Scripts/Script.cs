namespace Archean.Core.Models.Scripts;

public abstract class Script
{
    public bool HasStopped { get; protected set; }

    public abstract Task OnTickAsync();

    public void Stop()
    {
        HasStopped = true;
    }
}
