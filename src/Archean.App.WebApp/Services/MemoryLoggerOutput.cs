using Archean.Core;
using Archean.Hosting.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Archean.App.WebApp.Services;

public class MemoryLoggerOutput : ILoggerOutput, IDisposable
{
    private readonly RollingQueue<string> _logMessages;
    private readonly List<Func<Task>> _eventListeners = [];

    public IReadOnlyCollection<string> Messages => _logMessages;

    public MemoryLoggerOutput(int messageCapacity)
    {
        _logMessages = new RollingQueue<string>(messageCapacity, RollingQueueDirection.LastToFirst);
    }

    public void HandleLogEntry<TState>(LogEntry<TState> logEntry)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        _logMessages.Add(message);

        foreach (Func<Task> listener in _eventListeners.ToArray())
        {
            _ = Task.Run(listener); // Fire and forget, code invoking the current method cannot run async.
        }
    }

    public void SubscribeListener(Func<Task> eventListener)
    {
        _eventListeners.Add(eventListener);
    }

    public void UnsubscribeListener(Func<Task> eventListener)
    {
        _eventListeners.Remove(eventListener);
    }

    public void Clear()
    {
        _logMessages.Clear();
    }

    public void Dispose()
    {
        _eventListeners.Clear();
    }
}
