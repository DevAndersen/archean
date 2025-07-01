using Archean.Core.Models.Commands;

namespace Archean.Application.Models.Commands;

[Command("Test", Aliases = ["t"])]
public class TestCommand : Command
{
    private readonly ILogger<TestCommand> _logger;

    [CommandParameter(0, Required = true)]
    public int Number { get; set; }

    [CommandParameter(1, Required = true)]
    public string? Text { get; set; }

    public TestCommand(ILogger<TestCommand> logger)
    {
        _logger = logger;
    }

    public override Task InvokeAsync()
    {
        _logger.LogInformation("Test command invoked.");
        return Task.CompletedTask;
    }
}
