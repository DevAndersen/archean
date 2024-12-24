using Archean.Core.Models.Events;

namespace Archean.Core.Services.Events;

public interface IEventBus
{
    public Task InvokeEventAsync(Event eventArgs);

    public void Subscribe<TEvent>(Action<TEvent> action, EventPriority? priority) where TEvent : Event;

    public void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority? priority) where TEvent : Event;

    public void Unsubscribe<TEvent>(Delegate del) where TEvent : Event;

    public void Unsubscribe(Type eventType, Delegate del);
}
