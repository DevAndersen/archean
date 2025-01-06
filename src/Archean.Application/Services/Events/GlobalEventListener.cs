namespace Archean.Application.Services.Events;

public sealed class GlobalEventListener : IGlobalEventListener
{
    private readonly IGlobalEventBus globalEventBus;
    private readonly List<(Type EventType, Delegate EventDelegate)> eventDictionary = [];

    public GlobalEventListener(IGlobalEventBus globalEventBus)
    {
        this.globalEventBus = globalEventBus;
    }

    public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : Event
    {
        Subscribe(action, default);
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event
    {
        Subscribe(action, default);
    }

    public void Subscribe<TEvent>(Action<TEvent> action, EventPriority priority) where TEvent : Event
    {
        eventDictionary.Add((typeof(TEvent), action));
        globalEventBus.Subscribe(action, priority);
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority) where TEvent : Event
    {
        eventDictionary.Add((typeof(TEvent), action));
        globalEventBus.Subscribe(action, priority);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : Event
    {
        eventDictionary.Remove((typeof(TEvent), action));
        globalEventBus.Unsubscribe<TEvent>(action);
    }

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event
    {
        eventDictionary.Remove((typeof(TEvent), action));
        globalEventBus.Unsubscribe<TEvent>(action);
    }

    public void Dispose()
    {
        foreach ((Type eventType, Delegate eventDelegate) in eventDictionary)
        {
            globalEventBus?.Unsubscribe(eventType, eventDelegate);
        }
    }
}
