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

    private readonly Dictionary<string, Type> _nameRegistrations = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Type, CommandRegistration> _registrations = [];

    public CommandRegistry(ILogger<CommandRegistry> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public bool RegisterCommand(Type commandType, bool useDefaultNames, string[]? additionalNames = null)
    {
        if (!commandType.IsAssignableTo(typeof(ICommand)))
        {
            _logger.LogWarning("Failed to register command of type {commandTypeName}, type cannot be assigned to {commandInterfaceTypeName}",
                commandType.FullName,
                typeof(ICommand).FullName);

            return false;
        }

        CommandAttribute? commandAttribute = commandType.GetCustomAttribute<CommandAttribute>();
        if (commandAttribute == null)
        {
            _logger.LogWarning("Failed to register command of type {commandTypeName}, type is not decorated with {attributeTypeName}",
                commandType.FullName,
                typeof(CommandAttribute).FullName);

            return false;
        }

        CommandParameter[] parameters = [.. DetermineCommandParameters(commandType)];
        CommandRegistration registration = new CommandRegistration(commandType, parameters, commandAttribute.Name);

        if (!_registrations.TryAdd(commandType, registration))
        {
            _logger.LogWarning("Attempted to register already registered command of type {commandTypeName}",
                typeof(CommandAttribute).FullName);

            return false;
        }

        if (useDefaultNames)
        {
            RegisterCommandName(commandType, commandAttribute.Name);

            if (commandAttribute.Aliases != null)
            {
                foreach (string alias in commandAttribute.Aliases)
                {
                    RegisterCommandName(commandType, alias);
                }
            }
        }

        if (additionalNames != null)
        {
            foreach (string additionalName in additionalNames)
            {
                RegisterCommandName(commandType, additionalName);
            }
        }

        return true;
    }

    private bool RegisterCommandName(Type commandType, string? commandName)
    {
        if (string.IsNullOrWhiteSpace(commandName))
        {
            _logger.LogWarning("Failed to register empty or whitespace command name for command type {commandTypeName}",
                commandType.FullName);

            return false;
        }

        if (!_nameRegistrations.TryAdd(commandName, commandType))
        {
            _logger.LogWarning("Failed to register command name {commandName} for command of type {commandTypeName}, name already in use",
                commandName,
                commandType.FullName);

            return false;
        }

        return true;
    }

    public bool TryGetCommand(ReadOnlySpan<char> commandName, [NotNullWhen(true)] out ICommand? command, [NotNullWhen(true)] out CommandParameter[]? parameters)
    {
        if (_nameRegistrations.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(commandName, out Type? commandType))
        {
            if (_registrations.TryGetValue(commandType, out CommandRegistration? registration))
            {
                command = _serviceProvider.GetService(registration.Type) as ICommand;
                parameters = registration.Parameters;
                return command != null;
            }
        }

        command = null;
        parameters = null;
        return false;
    }

    public IEnumerable<CommandRegistration> GetCommands()
    {
        return _registrations.Values.OrderBy(x => x.Name);
    }

    private static IEnumerable<CommandParameter> DetermineCommandParameters(Type type)
    {
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            CommandParameterAttribute? attribute = property.GetCustomAttribute<CommandParameterAttribute>();
            if (attribute != null)
            {
                yield return new CommandParameter(property, attribute);
            }
        }
    }
}
