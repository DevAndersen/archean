﻿using Archean.Commands.Services;
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
        // Arrange
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Act
        bool wasCommandRegistered = commandRegistry.RegisterCommand(typeof(TestCommand), true);

        // Assert
        Assert.True(wasCommandRegistered);
    }

    [Fact]
    public void Register_CommandAlreadyInDictionary_ReturnsFalse()
    {
        // Arrange
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Act
        commandRegistry.RegisterCommand(typeof(TestCommand), true);
        bool wasCommandRegistered = commandRegistry.RegisterCommand(typeof(TestCommand), true);

        // Assert
        Assert.False(wasCommandRegistered);
    }

    [Fact]
    public void TryGetCommand_CommandName_CommandFound()
    {
        // Arrange
        _mockedServiceProvider.GetService(typeof(TestCommand)).Returns(new TestCommand());
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Act
        commandRegistry.RegisterCommand(typeof(TestCommand), true);
        bool wasCommandFound = commandRegistry.TryGetCommand(nameof(TestCommand), out Command? command, out _);

        // Assert
        Assert.True(wasCommandFound);
        Assert.True(command is TestCommand);
    }

    [Fact]
    public void TryGetCommand_CommandAliases_CommandFound()
    {
        // Arrange
        _mockedServiceProvider.GetService(typeof(TestCommand)).Returns(new TestCommand());
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Act
        commandRegistry.RegisterCommand(typeof(TestCommand), true);
        bool foundByAlias1 = commandRegistry.TryGetCommand("t", out Command? commandByAlias1, out _);
        bool foundByAlias2 = commandRegistry.TryGetCommand("123", out Command? commandByAlias2, out _);

        // Assert
        Assert.True(foundByAlias1);
        Assert.True(foundByAlias2);
        Assert.True(commandByAlias1 is TestCommand);
        Assert.True(commandByAlias2 is TestCommand);
    }

    [Fact]
    public void TryGetCommand_IncorrectName_CommandNotFound()
    {
        // Arrange
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Act
        bool wasCommandFound = commandRegistry.TryGetCommand(nameof(TestCommand), out Command? command, out _);

        // Assert
        Assert.False(wasCommandFound);
        Assert.Null(command);
    }

    [Fact]
    public void TryGetCommand_CommandNotInServiceProvider_CommandFound()
    {
        // Arrange
        ICommandRegistry commandRegistry = new CommandRegistry(_mockedLogger, _mockedServiceProvider);

        // Act
        commandRegistry.RegisterCommand(typeof(TestCommand), true);
        bool wasCommandFound = commandRegistry.TryGetCommand(nameof(TestCommand), out Command? command, out _);

        // Assert
        Assert.False(wasCommandFound);
        Assert.Null(command);
    }

    [Command(nameof(TestCommand), Aliases = ["t", "123"])]
    private class TestCommand : Command
    {
        public override Task InvokeAsync() => Task.CompletedTask;
    }
}
