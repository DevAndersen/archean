using Archean.Core.Models.Events;
using Archean.Core.Services.Events;
using System.Collections.Concurrent;

namespace Archean.Application.Services.Events;

public class EventBus : IGlobalEventBus
{
    private readonly SemaphoreSlim subscriptionSemaphore = new SemaphoreSlim(1);
    private readonly ConcurrentDictionary<Type, SortedDictionary<int, List<Delegate>>> eventDictionary = [];

    public async Task InvokeEventAsync(Event eventArgs)
    {
        Type eventType = eventArgs.GetType();
        if (eventDictionary.TryGetValue(eventType, out SortedDictionary<int, List<Delegate>>? eventList) && eventList.Count != 0)
        {
            Type expectedType = typeof(Action<>).MakeGenericType(eventType);
            Type expectedAsyncType = typeof(Func<,>).MakeGenericType([eventType, typeof(Task)]);
            object[] argsArray = [eventArgs];

            foreach ((int index, List<Delegate> itemList) in eventList)
            {
                foreach (Delegate item in itemList)
                {
                    if (item.GetType() == expectedType)
                    {
                        item.Method.Invoke(item.Target, argsArray);
                    }
                    else if (item.GetType() == expectedAsyncType)
                    {
                        await (Task)item.Method.Invoke(item.Target, argsArray)!;
                    }

                    if (eventArgs.IsCancelled)
                    {
                        return;
                    }
                }
            }
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> action, int? priority) where TEvent : Event
    {
        SubscribeDelegate<TEvent>(action, priority);
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> action, int? priority) where TEvent : Event
    {
        SubscribeDelegate<TEvent>(action, priority);
    }

    private void SubscribeDelegate<TEvent>(Delegate del, int? priority)
    {
        try
        {
            subscriptionSemaphore.Wait();

            int priorityValue = priority ?? 0;

            Type eventArgsType = typeof(TEvent);
            if (!eventDictionary.TryGetValue(eventArgsType, out SortedDictionary<int, List<Delegate>>? eventLists))
            {
                eventLists = eventDictionary[eventArgsType] = [];
            }

            if (!eventLists.TryGetValue(priorityValue, out List<Delegate>? eventsForIndex))
            {
                eventsForIndex = eventLists[priorityValue] = [];
            }

            eventsForIndex.Add(del);
        }
        finally
        {
            subscriptionSemaphore.Release();
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
            subscriptionSemaphore.Wait();

            if (eventDictionary.TryGetValue(eventType, out SortedDictionary<int, List<Delegate>>? eventList))
            {
                foreach ((int _, List<Delegate> list) in eventList)
                {
                    list.RemoveAll(x => x == del);
                }
            }
        }
        finally
        {
            subscriptionSemaphore.Release();
        }
    }
}
