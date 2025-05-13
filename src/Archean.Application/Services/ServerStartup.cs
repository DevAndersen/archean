using Archean.Application.Services.Commands;
using Archean.Core.Services;
using Archean.Core.Services.Commands;
using System.Reflection;

namespace Archean.Application.Services;

public class ServerStartup
{
    private readonly IBlockDictionary _blockDictionary;
    private readonly ICommandDictionary _commandDictionary;
    private readonly CommandRegistrations _commandRegistrations;
    private readonly ILogger<ServerStartup> _logger;

    public ServerStartup(
        IBlockDictionary blockDictionary,
        ICommandDictionary commandDictionary,
        CommandRegistrations commandRegistrations,
        ILogger<ServerStartup> logger)
    {
        _blockDictionary = blockDictionary;
        _commandDictionary = commandDictionary;
        _commandRegistrations = commandRegistrations;
        _logger = logger;
    }

    /// <summary>
    /// Invokes setup operations to perpare services.
    /// </summary>
    public void PerformSetup()
    {
        _blockDictionary.RegisterStandardBlocks();
        _blockDictionary.RegisterCustomAliases();
        RegisterCommands();
    }

    private void RegisterCommands()
    {
        int commandCount = 0;

        MethodInfo? registerMethod = typeof(ICommandDictionary)
            .GetMethod(nameof(ICommandDictionary.Register));

        if (registerMethod == null)
        {
            _logger.LogError("Failed to find ICommandDictionary's register method");
            return;
        }

        foreach (Type commandType in _commandRegistrations.CommandTypes)
        {
            object? result = registerMethod.MakeGenericMethod(commandType)
                .Invoke(_commandDictionary, []);

            if (result is not bool wasCommandRegistered)
            {
                _logger.LogError("Attempt to register command {commandType} returned unexpected type {returnType}",
                    commandType.FullName,
                    result);

                return;
            }

            if (wasCommandRegistered)
            {
                commandCount++;
            }
        }

        _logger.LogDebug("Registered {commandCount} commands", commandCount);
    }
}
