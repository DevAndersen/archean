using Archean.Core.Models.Events;

namespace Archean.Core.Services.Events;

public interface IEventListener : IDisposable
{
    void Subscribe<TEvent>(Action<TEvent> action) where TEvent : Event;

    void Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event;

    void Subscribe<TEvent>(Action<TEvent> action, EventPriority priority) where TEvent : Event;

    void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority) where TEvent : Event;

    void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : Event;

    void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event;
}
