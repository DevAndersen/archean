﻿using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Application.Services.Commands;

public class CommandDictionary : ICommandDictionary
{
    private readonly ILogger<CommandDictionary> _logger;
    private readonly Dictionary<string, Type> _dictionary;
    private readonly IServiceProvider _serviceProvider;

    public CommandDictionary(ILogger<CommandDictionary> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _dictionary = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        _serviceProvider = serviceProvider;
    }

    public bool Register<TCommand>() where TCommand : ICommand
    {
        if (string.IsNullOrWhiteSpace(TCommand.CommandName))
        {
            _logger.LogWarning("Failed to register empty or whitespace command name for command type {commandTypeName}",
                typeof(TCommand).FullName);

            return false;
        }

        if (!_dictionary.TryAdd(TCommand.CommandName, typeof(TCommand)))
        {
            _logger.LogWarning("Failed to register command name {commandName} for command type {commandTypeName}, command name/alias already in use",
                TCommand.CommandName,
                typeof(TCommand).FullName);

            return false;
        }

        _logger.LogTrace("Succesfully registered command name {commandName} for command type {commandTypeName}",
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

    private bool RegisterAlias<TCommand>(string alias) where TCommand : ICommand
    {
        if (string.IsNullOrWhiteSpace(TCommand.CommandName))
        {
            _logger.LogWarning("Failed to register empty or whitespace command alias for command type {commandTypeName}",
                typeof(TCommand).FullName);

            return false;
        }

        if (!_dictionary.TryAdd(alias, typeof(TCommand)))
        {
            _logger.LogWarning("Failed to register command name {commandAlias} for command type {commandTypeName}, command name/alias already in use",
                alias,
                typeof(TCommand).FullName);

            return false;
        }

        _logger.LogTrace("Succesfully registered command alias {commandAlias} for command type {commandTypeName}",
                alias,
                typeof(TCommand).FullName);

        return true;
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
}
