namespace Archean.Core.Services.Events;

/// <summary>
/// Defines behavior for subscribing delegates to scoped and global events, including automatic unsubscription when the instance is disposed.
/// </summary>
public interface IScopedEventListener : IEventListener
{
}
