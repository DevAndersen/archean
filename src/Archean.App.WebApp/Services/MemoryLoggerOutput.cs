using Archean.Hosting.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Archean.App.WebApp.Services;

public class MemoryLoggerOutput : ILoggerOutput
{
    private readonly LinkedList<string> _logMessages = [];

    public IReadOnlyCollection<string> Messages => _logMessages;

    public void HandleLogEntry<TState>(LogEntry<TState> logEntry)
    {
        if (_logMessages.Count == 5) // Todo: Make this configurable.
        {
            _logMessages.RemoveFirst();
        }

        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        _logMessages.AddLast(message);
    }
}
