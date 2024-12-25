namespace Archean.Application.Services.Events;

public class EventListener : IEventListener
{
    private readonly IEventBus scopedEventBus;
    private readonly IGlobalEventBus globalEventBus;
    private readonly List<(Type EventType, Delegate EventDelegate)> eventDictionary = [];

    public EventListener(IEventBus scopedEventBus, IGlobalEventBus globalEventBus)
    {
        this.scopedEventBus = scopedEventBus;
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
        scopedEventBus.Subscribe(action, priority);
        globalEventBus.Subscribe(action, priority);
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority) where TEvent : Event
    {
        eventDictionary.Add((typeof(TEvent), action));
        scopedEventBus.Subscribe(action, priority);
        globalEventBus.Subscribe(action, priority);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : Event
    {
        eventDictionary.Remove((typeof(TEvent), action));
        scopedEventBus.Unsubscribe<TEvent>(action);
        globalEventBus.Unsubscribe<TEvent>(action);
    }

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event
    {
        eventDictionary.Remove((typeof(TEvent), action));
        scopedEventBus.Unsubscribe<TEvent>(action);
        globalEventBus.Unsubscribe<TEvent>(action);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach ((Type eventType, Delegate eventDelegate) in eventDictionary)
            {
                globalEventBus?.Unsubscribe(eventType, eventDelegate);
                scopedEventBus?.Unsubscribe(eventType, eventDelegate);
            }
        }
    }
}
