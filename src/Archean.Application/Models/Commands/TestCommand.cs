using Archean.Commands;
using Archean.Core.Models.Commands;

namespace Archean.Application.Models.Commands;

[Command("Test", Aliases = ["t"])]
public class TestCommand : ICommand
{
    private readonly ILogger<TestCommand> _logger;

    public TestCommand(ILogger<TestCommand> logger)
    {
        _logger = logger;
    }

    public Task InvokeAsync()
    {
        _logger.LogInformation("Test command invoked.");
        return Task.CompletedTask;
    }
}
