using Archean.Core.Models.Events;
using Archean.Core.Services.Events;

namespace Archean.Events.Services;

public sealed class ScopedEventListener : IScopedEventListener
{
    private readonly IScopedEventBus _scopedEventBus;
    private readonly IGlobalEventBus _globalEventBus;
    private readonly List<(Type EventType, Delegate EventDelegate)> _eventDictionary = [];

    public ScopedEventListener(IScopedEventBus scopedEventBus, IGlobalEventBus globalEventBus)
    {
        _scopedEventBus = scopedEventBus;
        _globalEventBus = globalEventBus;
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
        _eventDictionary.Add((typeof(TEvent), action));
        _scopedEventBus.Subscribe(action, priority);
        _globalEventBus.Subscribe(action, priority);
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority) where TEvent : Event
    {
        _eventDictionary.Add((typeof(TEvent), action));
        _scopedEventBus.Subscribe(action, priority);
        _globalEventBus.Subscribe(action, priority);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : Event
    {
        _eventDictionary.Remove((typeof(TEvent), action));
        _scopedEventBus.Unsubscribe<TEvent>(action);
        _globalEventBus.Unsubscribe<TEvent>(action);
    }

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event
    {
        _eventDictionary.Remove((typeof(TEvent), action));
        _scopedEventBus.Unsubscribe<TEvent>(action);
        _globalEventBus.Unsubscribe<TEvent>(action);
    }

    public void Dispose()
    {
        foreach ((Type eventType, Delegate eventDelegate) in _eventDictionary)
        {
            _globalEventBus?.Unsubscribe(eventType, eventDelegate);
            _scopedEventBus?.Unsubscribe(eventType, eventDelegate);
        }
    }
}
