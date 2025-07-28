using Archean.Core.Models.Events;

namespace Archean.Core.Services.Events;

/// <summary>
/// Defines behavior for subscribing delegates to events, including automatic unsubscription when the instance is disposed.
/// </summary>
/// <remarks>
/// This type should not be used directly. Instead, use <see cref="IGlobalEventListener"/> or <see cref="IScopedEventListener"/>.
/// </remarks>
public interface IEventListener : IDisposable
{
    /// <summary>
    /// Subscribe <paramref name="action"/> with <paramref name="priority"/> to be invoked synchronously when an <typeparamref name="TEvent"/> is invoked.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    void Subscribe<TEvent>(Action<TEvent> action, EventPriority priority = EventPriority.Default) where TEvent : Event;

    /// <summary>
    /// Subscribe <paramref name="action"/> with <paramref name="priority"/> to be invoked asynchronously when an <typeparamref name="TEvent"/> is invoked.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    void Subscribe<TEvent>(Func<TEvent, Task> action, EventPriority priority = EventPriority.Default) where TEvent : Event;

    /// <summary>
    /// Unsubscribe <paramref name="action"/> from being invoked synchronously when an <typeparamref name="TEvent"/> is invoked.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="action"></param>
    void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : Event;

    /// <summary>
    /// Unsubscribe <paramref name="action"/> from being invoked asynchronously when an <typeparamref name="TEvent"/> is invoked.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="action"></param>
    void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event;
}
