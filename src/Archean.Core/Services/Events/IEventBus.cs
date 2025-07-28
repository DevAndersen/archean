using Archean.Core.Models.Events;

namespace Archean.Core.Services.Events;

/// <summary>
/// Handles subscription to, and execution of, events.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Invoke a new <paramref name="eventArgs"/> event, invoking all delegates subscribed to the specified event type.
    /// Delegates are invoked according to their priority, defined then subscribing a delegate to an event.
    /// Delegates can cancel the event, causing the event to not be invoked for other subscribers.
    /// </summary>
    /// <param name="eventArgs"></param>
    /// <returns></returns>
    Task InvokeEventAsync(Event eventArgs);

    /// <summary>
    /// Subscribe <paramref name="action"/> as a synchronous subscriber for <typeparamref name="TEvent"/> events, with <paramref name="priority"/>.
    /// </summary>
    /// <remarks>
    /// Event subscriptions use exact type matches. Subscribing to an event type will not result in also subscribing to events of child event types.
    /// </remarks>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    void Subscribe<TEvent>(Action<TEvent> action, EventPriority priority = EventPriority.Default) where TEvent : Event;

    /// <summary>
    /// Subscribe <paramref name="action"/> as an asynchronous subscriber for <typeparamref name="TEvent"/> events, with <paramref name="priority"/>.
    /// </summary>
    /// <remarks>
    /// Event subscriptions use exact type matches. Subscribing to an event type will not result in also subscribing to events of child event types.
    /// </remarks>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority = EventPriority.Default) where TEvent : Event;

    /// <summary>
    /// Unsubscribe <paramref name="del"/> from <typeparamref name="TEvent"/> events.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="del"></param>
    void Unsubscribe<TEvent>(Delegate del) where TEvent : Event;

    /// <summary>
    /// Unsubscribe <paramref name="del"/> from events of type <paramref name="eventType"/>.
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="del"></param>
    void Unsubscribe(Type eventType, Delegate del);
}
