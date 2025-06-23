using Archean.Core.Models.Events;
using Archean.Core.Services.Events;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Archean.Events.Services;

public class EventBus : IGlobalEventBus, IScopedEventBus
{
    private readonly ReversePriorityComparer _reversePriorityComparer = new ReversePriorityComparer();
    private readonly SemaphoreSlim _subscriptionSemaphore = new SemaphoreSlim(1);
    private readonly ConcurrentDictionary<Type, SortedDictionary<EventPriority, List<Delegate>>> _eventDictionary = [];

    private readonly ILogger<EventBus> _logger;

    public EventBus(ILogger<EventBus> logger)
    {
        _logger = logger;
    }

    public async Task InvokeEventAsync(Event eventArgs)
    {
        Type eventType = eventArgs.GetType();
        if (_eventDictionary.TryGetValue(eventType, out SortedDictionary<EventPriority, List<Delegate>>? eventList) && eventList.Count != 0)
        {
            Type expectedType = typeof(Action<>).MakeGenericType(eventType);
            Type expectedAsyncType = typeof(Func<,>).MakeGenericType([eventType, typeof(Task)]);
            object[] argsArray = [eventArgs];

            foreach ((EventPriority _, List<Delegate> itemList) in eventList)
            {
                foreach (Delegate item in itemList)
                {
                    Type eventListenerType = item.GetType();
                    if (eventListenerType == expectedType)
                    {
                        item.Method.Invoke(item.Target, argsArray);
                    }
                    else if (eventListenerType == expectedAsyncType)
                    {
                        await (Task)item.Method.Invoke(item.Target, argsArray)!;
                    }
                    else
                    {
                        _logger.LogError("Unexpected {eventType} event listener for of type {eventListenerType}",
                            eventType.Name,
                            eventListenerType.FullName);
                    }

                    if (eventArgs.IsCancelled)
                    {
                        return;
                    }
                }
            }
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> action, EventPriority? priority) where TEvent : Event
    {
        SubscribeDelegate<TEvent>(action, priority);
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority? priority) where TEvent : Event
    {
        SubscribeDelegate<TEvent>(action, priority);
    }

    private void SubscribeDelegate<TEvent>(Delegate del, EventPriority? priority)
    {
        try
        {
            _subscriptionSemaphore.Wait();

            EventPriority priorityValue = priority ?? default;

            Type eventArgsType = typeof(TEvent);
            if (!_eventDictionary.TryGetValue(eventArgsType, out SortedDictionary<EventPriority, List<Delegate>>? eventLists))
            {
                eventLists = _eventDictionary[eventArgsType] = new SortedDictionary<EventPriority, List<Delegate>>(_reversePriorityComparer);
            }

            if (!eventLists.TryGetValue(priorityValue, out List<Delegate>? eventsForIndex))
            {
                eventsForIndex = eventLists[priorityValue] = [];
            }

            eventsForIndex.Add(del);
        }
        finally
        {
            _subscriptionSemaphore.Release();
        }
    }

    public void Unsubscribe<TEvent>(Delegate del) where TEvent : Event
    {
        Unsubscribe(typeof(TEvent), del);
    }

    public void Unsubscribe(Type eventType, Delegate del)
    {
        try
        {
            _subscriptionSemaphore.Wait();

            if (_eventDictionary.TryGetValue(eventType, out SortedDictionary<EventPriority, List<Delegate>>? eventList))
            {
                foreach ((EventPriority _, List<Delegate> list) in eventList)
                {
                    list.RemoveAll(x => x == del);
                }
            }
        }
        finally
        {
            _subscriptionSemaphore.Release();
        }
    }

    private class ReversePriorityComparer : IComparer<EventPriority>
    {
        public int Compare(EventPriority x, EventPriority y)
        {
            return -x.CompareTo(y);
        }
    }
}
