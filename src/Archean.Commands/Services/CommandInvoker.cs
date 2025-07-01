using Archean.Core.Models;
using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static System.MemoryExtensions;

namespace Archean.Commands.Services;

public class CommandInvoker : ICommandInvoker
{
    private readonly ICommandRegistry _commandRegistry;
    private readonly ILogger<CommandInvoker> _logger;

    public CommandInvoker(ICommandRegistry commandRegistry, ILogger<CommandInvoker> logger)
    {
        _commandRegistry = commandRegistry;
        _logger = logger;
    }

    public async Task<bool> TryInvokeCommandAsync(ReadOnlyMemory<char> commandText, IPlayer? invokingPlayer)
    {
        if (commandText.Span.IsWhiteSpace())
        {
            return false;
        }

        ReadOnlySpan<char> span = commandText.Span;
        SpanSplitEnumerator<char> splitRanges = span.Split(' ');

        splitRanges.MoveNext();
        ReadOnlySpan<char> commandName = span[splitRanges.Current];

        if (!_commandRegistry.TryGetCommand(commandName, out Command? command, out CommandParameter[]? parameters))
        {
            // Todo: Error response, unable to find command.
            return false;
        }

        foreach ((PropertyInfo property, CommandParameterAttribute parameter) in parameters)
        {
            if (TryParseParameterValue(property.PropertyType, span, ref splitRanges, out object? value))
            {
                property.SetValue(command, value);
            }
            else
            {
                // Todo: Error response, specify which parameter could not be parsed.
                return false;
            }
        }

        try
        {
            command.InvokingPlayer = invokingPlayer;
            await command.InvokeAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An exception was thrown when attempting to invoke command {commandText}",
                commandText.ToString());

            return false;
        }

        return true;
    }

    private static bool TryParseParameterValue(Type parameterType, ReadOnlySpan<char> span, ref SpanSplitEnumerator<char> ranges, [NotNullWhen(true)] out object? value)
    {
        if (!ranges.MoveNext())
        {
            value = null;
            return false;
        }

        ReadOnlySpan<char> slice = span[ranges.Current];

        if (parameterType == typeof(int) || parameterType == typeof(int?))
        {
            bool success = int.TryParse(slice, out int parsedValue);
            value = success ? parsedValue : null;
            return success;
        }

        if (parameterType == typeof(string))
        {
            int start = ranges.Current.Start.Value;

            // Iterate through all ranges.
            while (ranges.MoveNext()) { }

            int end = ranges.Current.End.Value;

            value = new string(span[start..end]);
            return true;
        }

        if (parameterType == typeof(bool) || parameterType == typeof(bool?))
        {
            bool success = bool.TryParse(slice, out bool parsedValue);
            value = success ? parsedValue : null;
            return success;
        }

        value = null;
        return false;
    }
}
