using Archean.Core.Models.Events;

namespace Archean.Core.Services.Events;

/// <summary>
/// Defines behavior for notifying subscribers about events.
/// </summary>
/// <remarks>
/// This type should not be used directly. Instead, use <see cref="IGlobalEventBus"/> or <see cref="IScopedEventBus"/>.
/// </remarks>
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
    /// <list type="bullet">
    /// <item>This method should not be used directly to subscribe to events. Instead, use <see cref="IEventListener"/>.</item>
    /// <item>Event subscriptions use exact type matches. Subscribing to an event type will not result in also subscribing to events of child event types.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    void Subscribe<TEvent>(Action<TEvent> action, EventPriority priority = EventPriority.Default) where TEvent : Event;

    /// <summary>
    /// Subscribe <paramref name="action"/> as an asynchronous subscriber for <typeparamref name="TEvent"/> events, with <paramref name="priority"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>This method should not be used directly to subscribe to events. Instead, use <see cref="IEventListener"/>.</item>
    /// <item>Event subscriptions use exact type matches. Subscribing to an event type will not result in also subscribing to events of child event types.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority = EventPriority.Default) where TEvent : Event;

    /// <summary>
    /// Unsubscribe <paramref name="del"/> from <typeparamref name="TEvent"/> events.
    /// </summary>
    /// <remarks>
    /// This method should not be used directly to unsubscribe from events. Instead, use <see cref="IEventListener"/>.
    /// </remarks>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="del"></param>
    void Unsubscribe<TEvent>(Delegate del) where TEvent : Event;

    /// <summary>
    /// Unsubscribe <paramref name="del"/> from events of type <paramref name="eventType"/>.
    /// </summary>
    /// <remarks>
    /// This method should not be used directly to unsubscribe from events. Instead, use <see cref="IEventListener"/>.
    /// </remarks>
    /// <param name="eventType"></param>
    /// <param name="del"></param>
    void Unsubscribe(Type eventType, Delegate del);
}
