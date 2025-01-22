using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Application.Services.Commands;

public class CommandDictionary : ICommandDictionary
{
    private readonly ILogger<CommandDictionary> logger;
    private readonly Dictionary<string, Type> dictionary;
    private readonly IServiceProvider serviceProvider;

    public CommandDictionary(ILogger<CommandDictionary> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        dictionary = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        this.serviceProvider = serviceProvider;
    }

    public bool Register<TCommand>() where TCommand : ICommand
    {
        if (string.IsNullOrWhiteSpace(TCommand.CommandName))
        {
            logger.LogWarning("Failed to register empty or whitespace command name for command type {commandTypeName}",
                typeof(TCommand).FullName);

            return false;
        }

        if (!dictionary.TryAdd(TCommand.CommandName, typeof(TCommand)))
        {
            logger.LogWarning("Failed to register command name {commandName} for command type {commandTypeName}, command name/alias already in use",
                TCommand.CommandName,
                typeof(TCommand).FullName);

            return false;
        }

        logger.LogTrace("Succesfully registered command name {commandName} for command type {commandTypeName}",
                TCommand.CommandName,
                typeof(TCommand).FullName);

        if (TCommand.CommandAliases?.Length > 0)
        {
            foreach (string alias in TCommand.CommandAliases)
            {
                RegisterAlias<TCommand>(alias);
            }
        }

        return true;
    }

    public bool RegisterAlias<TCommand>(string alias) where TCommand : ICommand
    {
        if (string.IsNullOrWhiteSpace(TCommand.CommandName))
        {
            logger.LogWarning("Failed to register empty or whitespace command alias for command type {commandTypeName}",
                typeof(TCommand).FullName);

            return false;
        }

        if (!dictionary.TryAdd(alias, typeof(TCommand)))
        {
            logger.LogWarning("Failed to register command name {commandAlias} for command type {commandTypeName}, command name/alias already in use",
                alias,
                typeof(TCommand).FullName);

            return false;
        }

        logger.LogTrace("Succesfully registered command alias {commandAlias} for command type {commandTypeName}",
                alias,
                typeof(TCommand).FullName);

        return true;
    }

    public bool TryGetCommand(ReadOnlySpan<char> commandName, [NotNullWhen(true)] out ICommand? command)
    {
        if (dictionary.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(commandName, out Type? commandType))
        {
            command = serviceProvider.GetService(commandType) as ICommand;
            return command != null;
        }
        command = null;
        return false;
    }
}
