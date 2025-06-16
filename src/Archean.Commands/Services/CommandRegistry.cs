using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Archean.Commands.Services;

public class CommandRegistry : ICommandRegistry
{
    private readonly ILogger<CommandRegistry> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, Type> _dictionary;

    public CommandRegistry(ILogger<CommandRegistry> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        _dictionary = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
    }

    public bool RegisterCommand(Type commandType)
    {
        if (!commandType.IsAssignableTo(typeof(ICommand)))
        {
            _logger.LogWarning("Failed to register command of type {commandTypeName}, type cannot be assigned to {commandInterfaceTypeName}",
                commandType.FullName,
                typeof(ICommand).FullName);

            return false;
        }

        if (GetCommandAttribute(commandType) == null)
        {
            _logger.LogWarning("Failed to register command of type {commandTypeName}, type is not decorated with {attributeTypeName}",
                commandType.FullName,
                typeof(CommandAttribute).FullName);

            return false;
        }

        string? commandName = GetCommandName(commandType);

        if (string.IsNullOrWhiteSpace(commandName))
        {
            _logger.LogWarning("Failed to register empty or whitespace command name for command type {commandTypeName}",
                commandType.FullName);

            return false;
        }

        if (!_dictionary.TryAdd(commandName, commandType))
        {
            _logger.LogWarning("Failed to register command name {commandName} for command type {commandTypeName}, command name/alias already in use",
                commandName,
                commandType.FullName);

            return false;
        }

        _logger.LogTrace("Succesfully registered command name {commandName} for command type {commandTypeName}",
                commandName,
                commandType.FullName);

        return true;
    }

    public void RegisterCommandAliases(Type commandType)
    {
        if (GetCommandAttribute(commandType) == null)
        {
            _logger.LogWarning("Failed to register aliases for command of type {commandTypeName}, type is not decorated with {attributeTypeName}",
                commandType.FullName,
                typeof(CommandAttribute).FullName);

            return;
        }

        string[]? aliases = GetCommandAliases(commandType);

        if (aliases is { Length: > 0 })
        {
            foreach (string alias in aliases)
            {
                RegisterCommandAlias(commandType, alias);
            }
        }
    }

    public bool RegisterCommandAlias(Type commandType, string alias)
    {
        if (!commandType.IsAssignableTo(typeof(ICommand)))
        {
            _logger.LogWarning("Failed to register command alias {commandAlias} for command type {commandTypeName}, type cannot be assigned to {commandInterfaceTypeName}",
                alias,
                commandType.FullName,
                typeof(ICommand).FullName);

            return false;
        }

        if (string.IsNullOrWhiteSpace(alias))
        {
            _logger.LogWarning("Failed to register empty or whitespace command alias for command type {commandTypeName}",
                commandType.FullName);

            return false;
        }

        if (!_dictionary.TryAdd(alias, commandType))
        {
            _logger.LogWarning("Failed to register command name {commandAlias} for command type {commandTypeName}, command name/alias already in use",
                alias,
                commandType.FullName);

            return false;
        }

        _logger.LogTrace("Succesfully registered command alias {commandAlias} for command type {commandTypeName}",
                alias,
                commandType.FullName);

        return true;
    }

    private static string? GetCommandName(Type commandType)
    {
        return GetCommandAttribute(commandType)?.Name;
    }

    private static string[]? GetCommandAliases(Type commandType)
    {
        return GetCommandAttribute(commandType)?.Aliases;
    }

    private static CommandAttribute? GetCommandAttribute(Type commandType)
    {
        return commandType.GetCustomAttribute<CommandAttribute>();
    }

    public bool TryGetCommand(ReadOnlySpan<char> commandName, [NotNullWhen(true)] out ICommand? command)
    {
        if (_dictionary.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(commandName, out Type? commandType))
        {
            command = _serviceProvider.GetService(commandType) as ICommand;
            return command != null;
        }
        command = null;
        return false;
    }

    public IEnumerable<ICommand> GetCommands()
    {
        foreach (Type value in _dictionary.Values)
        {
            if (_serviceProvider.GetService(value) is ICommand command)
            {
                yield return command;
            }
        }
    }
}
