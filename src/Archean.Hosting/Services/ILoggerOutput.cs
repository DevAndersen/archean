using Microsoft.Extensions.Logging.Abstractions;

namespace Archean.Hosting.Services;

public interface ILoggerOutput
{
    void HandleLogEntry<TState>(LogEntry<TState> logEntry);

    void SubscribeListener(Func<Task> eventListener);

    void UnsubscribeListener(Func<Task> eventListener);
}
