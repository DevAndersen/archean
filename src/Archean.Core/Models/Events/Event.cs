namespace Archean.Core.Models.Events;

public abstract class Event
{
    public bool IsCancelled { get; protected set; }

    /// <summary>
    /// Cancel the event, preventing further event propagation.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
    }
}
