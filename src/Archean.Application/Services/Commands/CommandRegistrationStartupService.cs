using Archean.Core.Services;
using Archean.Core.Services.Commands;
using System.Reflection;

namespace Archean.Application.Services.Commands;

public class CommandRegistrationStartupService : IStartupService
{
    private readonly ILogger<CommandRegistrationStartupService> _logger;
    private readonly CommandRegistrations _commandRegistrations;
    private readonly ICommandDictionary _commandDictionary;

    public CommandRegistrationStartupService(
        ILogger<CommandRegistrationStartupService> logger,
        CommandRegistrations commandRegistrations,
        ICommandDictionary commandDictionary)
    {
        _logger = logger;
        _commandRegistrations = commandRegistrations;
        _commandDictionary = commandDictionary;
    }

    public Task OnStartupAsync()
    {
        int commandCount = 0;

        MethodInfo? registerMethod = typeof(ICommandDictionary)
            .GetMethod(nameof(ICommandDictionary.Register));

        if (registerMethod == null)
        {
            _logger.LogError("Failed to find ICommandDictionary's register method");
            throw new NotImplementedException(); // Todo: Throw an appripriate exception.
        }

        foreach (Type commandType in _commandRegistrations.CommandTypes)
        {
            object? result = registerMethod.MakeGenericMethod(commandType)
                .Invoke(_commandDictionary, []);

            if (result is bool wasCommandRegistered)
            {
                if (wasCommandRegistered)
                {
                    commandCount++;
                }
            }
            else
            {
                _logger.LogError("Attempt to register command {commandType} returned unexpected type {returnType}",
                    commandType.FullName,
                    result);
            }
        }

        _logger.LogDebug("Registered {commandCount} commands", commandCount);

        return Task.CompletedTask;
    }
}
