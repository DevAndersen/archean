using Archean.Application.Services.Commands;
using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Archean.Tests.Commands;

public class CommandDictionaryTests
{
    private readonly ILogger<CommandDictionary> _mockedLogger;
    private readonly IServiceProvider _mockedServiceProvider;

    public CommandDictionaryTests()
    {
        _mockedLogger = Substitute.For<ILogger<CommandDictionary>>();
        _mockedServiceProvider = Substitute.For<IServiceProvider>();
    }

    [Fact]
    public void Register_EmptyDictionary_ReturnsTrue()
    {
        // Setup
        ICommandDictionary commandDictionary = new CommandDictionary(_mockedLogger, _mockedServiceProvider);

        // Action
        bool wasCommandRegistered = commandDictionary.Register<TestCommand>();

        // Assert
        Assert.True(wasCommandRegistered);
    }

    [Fact]
    public void Register_CommandAlreadyInDictionary_ReturnsFalse()
    {
        // Setup
        ICommandDictionary commandDictionary = new CommandDictionary(_mockedLogger, _mockedServiceProvider);

        // Action
        commandDictionary.Register<TestCommand>();
        bool wasCommandRegistered = commandDictionary.Register<TestCommand>();

        // Assert
        Assert.False(wasCommandRegistered);
    }

    [Fact]
    public void TryGetCommand_CommandName_CommandFound()
    {
        // Setup
        _mockedServiceProvider.GetService(typeof(TestCommand)).Returns(new TestCommand());
        ICommandDictionary commandDictionary = new CommandDictionary(_mockedLogger, _mockedServiceProvider);

        // Action
        commandDictionary.Register<TestCommand>();
        bool wasCommandFound = commandDictionary.TryGetCommand(TestCommand.CommandName, out ICommand? command);

        // Assert
        Assert.True(wasCommandFound);
        Assert.True(command is TestCommand);
    }

    [Fact]
    public void TryGetCommand_CommandAliases_CommandFound()
    {
        // Setup
        _mockedServiceProvider.GetService(typeof(TestCommand)).Returns(new TestCommand());
        ICommandDictionary commandDictionary = new CommandDictionary(_mockedLogger, _mockedServiceProvider);

        // Action
        commandDictionary.Register<TestCommand>();
        bool foundByAlias1 = commandDictionary.TryGetCommand(TestCommand.CommandAliases[0], out ICommand? commandByAlias1);
        bool foundByAlias2 = commandDictionary.TryGetCommand(TestCommand.CommandAliases[1], out ICommand? commandByAlias2);

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
        ICommandDictionary commandDictionary = new CommandDictionary(_mockedLogger, _mockedServiceProvider);

        // Action
        bool wasCommandFound = commandDictionary.TryGetCommand(TestCommand.CommandName, out ICommand? command);

        // Assert
        Assert.False(wasCommandFound);
        Assert.Null(command);
    }

    [Fact]
    public void TryGetCommand_CommandNotInServiceProvider_CommandFound()
    {
        // Setup
        ICommandDictionary commandDictionary = new CommandDictionary(_mockedLogger, _mockedServiceProvider);

        // Action
        commandDictionary.Register<TestCommand>();
        bool wasCommandFound = commandDictionary.TryGetCommand(TestCommand.CommandName, out ICommand? command);

        // Assert
        Assert.False(wasCommandFound);
        Assert.Null(command);
    }

    private class TestCommand : ICommand
    {
        public static string CommandName => nameof(TestCommand);

        public static string[] CommandAliases => ["t", "123"];

        public Task InvokeAsync() => Task.CompletedTask;
    }
}
