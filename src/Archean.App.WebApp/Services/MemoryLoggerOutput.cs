using Archean.Core;
using Archean.Hosting.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Archean.App.WebApp.Services;

public class MemoryLoggerOutput : ILoggerOutput
{
    private readonly FixedSizeQueue<string> _logMessages = new FixedSizeQueue<string>(5, FixedSizeQueueDirection.LastToFirst);

    public IReadOnlyCollection<string> Messages => _logMessages;

    public void HandleLogEntry<TState>(LogEntry<TState> logEntry)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        _logMessages.Add(message);
    }
}
