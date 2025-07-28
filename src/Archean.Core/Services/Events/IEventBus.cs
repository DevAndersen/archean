using Archean.Core.Models.Events;

namespace Archean.Core.Services.Events;

public interface IEventBus
{
    Task InvokeEventAsync(Event eventArgs);

    void Subscribe<TEvent>(Action<TEvent> action, EventPriority priority = EventPriority.Default) where TEvent : Event;

    void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority = EventPriority.Default) where TEvent : Event;

    void Unsubscribe<TEvent>(Delegate del) where TEvent : Event;

    void Unsubscribe(Type eventType, Delegate del);
}
