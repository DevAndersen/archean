using Archean.Core.Models.Events;

namespace Archean.Core.Services.Events;

public interface IEventListener : IDisposable
{
    public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : Event;

    public void Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event;

    public void Subscribe<TEvent>(Action<TEvent> action, EventPriority priority) where TEvent : Event;

    public void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority) where TEvent : Event;

    public void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : Event;

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event;
}