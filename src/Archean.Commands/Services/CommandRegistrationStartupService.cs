using Archean.Core.Models.Commands;
using Archean.Core.Services;
using Archean.Core.Services.Commands;
using Microsoft.Extensions.Logging;

namespace Archean.Commands.Services;

public class CommandRegistrationStartupService : IStartupService
{
    private readonly IEnumerable<Command> _commands;
    private readonly ICommandRegistry _commandRegistry;
    private readonly ILogger<CommandRegistrationStartupService> _logger;

    public CommandRegistrationStartupService(
        IEnumerable<Command> commands,
        ICommandRegistry commandRegistry,
        ILogger<CommandRegistrationStartupService> logger)
    {
        _commands = commands;
        _commandRegistry = commandRegistry;
        _logger = logger;
    }

    public Task OnStartupAsync()
    {
        foreach (Command command in _commands)
        {
            _commandRegistry.RegisterCommand(command.GetType(), true);
        }

        _logger.LogDebug("Registered {commandCount} commands", _commands.Count());

        return Task.CompletedTask;
    }
}
