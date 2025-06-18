using Archean.Commands;
using Archean.Commands.Services;
using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Archean.Tests.Commands;

public class CommandRegistryTests
{
    private readonly ILogger<CommandRegistry> _mockedLogger;
    private readonly IServiceProvider _mockedServiceProvider;

    public CommandRegistryTests()
    {
        _mockedLogger = Substitute.For<ILogger<CommandRegistry>>();
        _mockedServiceProvider = Substitute.For<IServiceProvider>();
    }

    [Fact]
    public void Register_EmptyDictionary_ReturnsTrue()
    {
        // Setup
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        bool wasCommandRegistered = commandRegistry.RegisterCommand(typeof(TestCommand), true);

        // Assert
        Assert.True(wasCommandRegistered);
    }

    [Fact]
    public void Register_CommandAlreadyInDictionary_ReturnsFalse()
    {
        // Setup
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        commandRegistry.RegisterCommand(typeof(TestCommand), true);
        bool wasCommandRegistered = commandRegistry.RegisterCommand(typeof(TestCommand), true);

        // Assert
        Assert.False(wasCommandRegistered);
    }

    [Fact]
    public void TryGetCommand_CommandName_CommandFound()
    {
        // Setup
        _mockedServiceProvider.GetService(typeof(TestCommand)).Returns(new TestCommand());
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        commandRegistry.RegisterCommand(typeof(TestCommand), true);
        bool wasCommandFound = commandRegistry.TryGetCommand(nameof(TestCommand), out ICommand? command, out _);

        // Assert
        Assert.True(wasCommandFound);
        Assert.True(command is TestCommand);
    }

    [Fact]
    public void TryGetCommand_CommandAliases_CommandFound()
    {
        // Setup
        _mockedServiceProvider.GetService(typeof(TestCommand)).Returns(new TestCommand());
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        commandRegistry.RegisterCommand(typeof(TestCommand), true);
        bool foundByAlias1 = commandRegistry.TryGetCommand("t", out ICommand? commandByAlias1, out _);
        bool foundByAlias2 = commandRegistry.TryGetCommand("123", out ICommand? commandByAlias2, out _);

        // Assert
        Assert.True(foundByAlias1);
        Assert.True(foundByAlias2);
        Assert.True(commandByAlias1 is TestCommand);
        Assert.True(commandByAlias2 is TestCommand);
    }

    [Fact]
    public void TryGetCommand_IncorrectName_CommandNotFound()
    {
        // Setup
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        bool wasCommandFound = commandRegistry.TryGetCommand(nameof(TestCommand), out ICommand? command, out _);

        // Assert
        Assert.False(wasCommandFound);
        Assert.Null(command);
    }

    [Fact]
    public void TryGetCommand_CommandNotInServiceProvider_CommandFound()
    {
        // Setup
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        commandRegistry.RegisterCommand(typeof(TestCommand), true);
        bool wasCommandFound = commandRegistry.TryGetCommand(nameof(TestCommand), out ICommand? command, out _);

        // Assert
        Assert.False(wasCommandFound);
        Assert.Null(command);
    }

    [Command(nameof(TestCommand), Aliases = ["t", "123"])]
    private class TestCommand : ICommand
    {
        public Task InvokeAsync() => Task.CompletedTask;
    }
}
