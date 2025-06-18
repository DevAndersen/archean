using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static System.MemoryExtensions;

namespace Archean.Commands.Services;

public class CommandInvoker : ICommandInvoker
{
    private readonly ICommandRegistry _commandRegistry;

    public CommandInvoker(ICommandRegistry commandRegistry)
    {
        _commandRegistry = commandRegistry;
    }

    public async Task<bool> TryInvokeCommandAsync(ReadOnlyMemory<char> commandText)
    {
        if (commandText.Span.IsWhiteSpace())
        {
            return false;
        }

        ReadOnlySpan<char> span = commandText.Span;
        SpanSplitEnumerator<char> splitRanges = span.Split(' ');

        splitRanges.MoveNext();
        ReadOnlySpan<char> commandName = span[splitRanges.Current];

        if (!_commandRegistry.TryGetCommand(commandName, out ICommand? command, out CommandParameter[]? parameters))
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

        await command.InvokeAsync();

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

        value = null;
        return false;
    }
}
