using Archean.Core.Models.Commands;

namespace Archean.Application.Models.Commands;

public class TestCommand : ICommand
{
    public static string CommandName => "Test";

    public static string[]? CommandAliases => ["t"];

    private readonly ILogger<TestCommand> logger;

    public TestCommand(ILogger<TestCommand> logger)
    {
        this.logger = logger;
    }

    public Task InvokeAsync()
    {
        logger.LogInformation("Test command invoked.");
        return Task.CompletedTask;
    }
}
