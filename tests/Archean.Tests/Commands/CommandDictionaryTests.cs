using Archean.Commands;
using Archean.Commands.Services;
using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Archean.Tests.Commands;

public class CommandDictionaryTests
{
    private readonly ILogger<CommandRegistry> _mockedLogger;
    private readonly IServiceProvider _mockedServiceProvider;

    public CommandDictionaryTests()
    {
        _mockedLogger = Substitute.For<ILogger<CommandRegistry>>();
        _mockedServiceProvider = Substitute.For<IServiceProvider>();
    }

    [Fact]
    public void Register_EmptyDictionary_ReturnsTrue()
    {
        // Setup
        ICommandRegistry commandDictionary = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        bool wasCommandRegistered = commandDictionary.RegisterCommand(typeof(TestCommand));

        // Assert
        Assert.True(wasCommandRegistered);
    }

    [Fact]
    public void Register_CommandAlreadyInDictionary_ReturnsFalse()
    {
        // Setup
        ICommandRegistry commandDictionary = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        commandDictionary.RegisterCommand(typeof(TestCommand));
        bool wasCommandRegistered = commandDictionary.RegisterCommand(typeof(TestCommand));

        // Assert
        Assert.False(wasCommandRegistered);
    }

    [Fact]
    public void TryGetCommand_CommandName_CommandFound()
    {
        // Setup
        _mockedServiceProvider.GetService(typeof(TestCommand)).Returns(new TestCommand());
        ICommandRegistry commandDictionary = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        commandDictionary.RegisterCommand(typeof(TestCommand));
        bool wasCommandFound = commandDictionary.TryGetCommand(nameof(TestCommand), out ICommand? command);

        // Assert
        Assert.True(wasCommandFound);
        Assert.True(command is TestCommand);
    }

    [Fact]
    public void TryGetCommand_CommandAliases_CommandFound()
    {
        // Setup
        _mockedServiceProvider.GetService(typeof(TestCommand)).Returns(new TestCommand());
        ICommandRegistry commandDictionary = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        commandDictionary.RegisterCommandAliases(typeof(TestCommand));
        bool foundByAlias1 = commandDictionary.TryGetCommand("t", out ICommand? commandByAlias1);
        bool foundByAlias2 = commandDictionary.TryGetCommand("123", out ICommand? commandByAlias2);

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
        ICommandRegistry commandDictionary = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        bool wasCommandFound = commandDictionary.TryGetCommand(nameof(TestCommand), out ICommand? command);

        // Assert
        Assert.False(wasCommandFound);
        Assert.Null(command);
    }

    [Fact]
    public void TryGetCommand_CommandNotInServiceProvider_CommandFound()
    {
        // Setup
        ICommandRegistry commandDictionary = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Action
        commandDictionary.RegisterCommand(typeof(TestCommand));
        bool wasCommandFound = commandDictionary.TryGetCommand(nameof(TestCommand), out ICommand? command);

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
