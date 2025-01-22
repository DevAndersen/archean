using Archean.Application.Services.Commands;
using Archean.Core.Services;
using Archean.Core.Services.Commands;
using System.Reflection;

namespace Archean.Application.Services;

public class ServerStartup
{
    private readonly IBlockDictionary blockDictionary;
    private readonly ICommandDictionary commandDictionary;
    private readonly CommandRegistrations commandRegistrations;
    private readonly ILogger<ServerStartup> logger;

    public ServerStartup(
        IBlockDictionary blockDictionary,
        ICommandDictionary commandDictionary,
        CommandRegistrations commandRegistrations,
        ILogger<ServerStartup> logger)
    {
        this.blockDictionary = blockDictionary;
        this.commandDictionary = commandDictionary;
        this.commandRegistrations = commandRegistrations;
        this.logger = logger;
    }

    /// <summary>
    /// Invokes setup operations to perpare services.
    /// </summary>
    public void PerformSetup()
    {
        blockDictionary.RegisterStandardBlocks();
        blockDictionary.RegisterCustomAliases();
        RegisterCommands();
    }

    private void RegisterCommands()
    {
        int commandCount = 0;

        MethodInfo? registerMethod = typeof(ICommandDictionary)
            .GetMethod(nameof(ICommandDictionary.Register));

        if (registerMethod == null)
        {
            logger.LogError("Failed to find ICommandDictionary's register method");
            return;
        }

        foreach (Type commandType in commandRegistrations.CommandTypes)
        {
            object? result = registerMethod.MakeGenericMethod(commandType)
                .Invoke(commandDictionary, []);

            if (result is not bool wasCommandRegistered)
            {
                logger.LogError("Attempt to register command {commandType} returned unexpected type {returnType}",
                    commandType.FullName,
                    result);

                return;
            }

            if (wasCommandRegistered)
            {
                commandCount++;
            }
        }

        logger.LogDebug("Registered {commandCount} commands", commandCount);
    }
}
