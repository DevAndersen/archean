using Microsoft.Extensions.Logging.Abstractions;

namespace Archean.Hosting.Services;

public interface ILoggerOutput
{
    void HandleLogEntry<TState>(LogEntry<TState> logEntry);
}
