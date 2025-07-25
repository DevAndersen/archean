using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Archean.Hosting.Services;

public class ArcheanConsoleLoggerFormatter : ConsoleFormatter, IDisposable
{
    private readonly IEnumerable<ILoggerOutput> _loggerOutputs;
    private readonly IDisposable? _optionsReloadToken;
    private ConsoleFormatterOptions _formatterOptions;

    public ArcheanConsoleLoggerFormatter(IOptionsMonitor<ConsoleFormatterOptions> options, IEnumerable<ILoggerOutput> loggerOutputs)
        : base(nameof(ArcheanConsoleLoggerFormatter))
    {
        _formatterOptions = options.CurrentValue;
        _optionsReloadToken = options.OnChange(options => _formatterOptions = options);
        _loggerOutputs = loggerOutputs;
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (message == null)
        {
            return;
        }

        // Timestamp
        textWriter.Write('[');
        WriteTimestamp(textWriter, _formatterOptions);
        textWriter.Write("] ");

        // Log level
        bool useColors = !Console.IsOutputRedirected;
        WriteLogLevel(textWriter, logEntry.LogLevel, useColors);
        textWriter.Write(": ");

        // Message
        WriteMessage(textWriter, message, logEntry);
        textWriter.WriteLine();

        foreach (ILoggerOutput loggerOutput in _loggerOutputs)
        {
            loggerOutput.HandleLogEntry(logEntry);
        }
    }

    private static void WriteTimestamp(TextWriter textWriter, ConsoleFormatterOptions formatterOptions)
    {
        DateTime now = formatterOptions.UseUtcTimestamp
            ? DateTime.UtcNow
            : DateTime.Now;

        textWriter.Write(now.ToString(formatterOptions.TimestampFormat ?? "HH\\:mm\\:ss"));
    }

    private static void WriteLogLevel(TextWriter textWriter, LogLevel logLevel, bool useColors)
    {
        string levelText = logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            LogLevel.None => "none",
            _ => logLevel.ToString()
        };

        if (useColors)
        {
            string colors = logLevel switch
            {
                LogLevel.Trace => "\e[40;37m",
                LogLevel.Debug => "\e[40;95m",
                LogLevel.Information => "\e[40;32m",
                LogLevel.Warning => "\e[40;93m",
                LogLevel.Error => "\e[41;30m",
                LogLevel.Critical => "\e[41;97m",
                LogLevel.None => string.Empty,
                _ => string.Empty
            };

            textWriter.Write(colors);
            textWriter.Write(levelText);
            textWriter.Write("\e[m");
        }
        else
        {
            textWriter.Write(levelText);
        }
    }

    private static void WriteMessage<TState>(TextWriter writer, string message, LogEntry<TState> logEntry)
    {
        writer.Write(message);
        if (logEntry.Exception != null)
        {
            writer.Write(Environment.NewLine);
            writer.Write(logEntry.Exception.StackTrace);
        }
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}
