namespace Archean.Core.Services.Events;

/// <summary>
/// Defines behavior for subscribing delegates to global events, including automatic unsubscription when the instance is disposed.
/// </summary>
public interface IGlobalEventListener : IEventListener
{
}
