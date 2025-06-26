using Archean.Commands.Services;
using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Archean.Tests.Commands;

public class CommandInvokerTests
{
    private readonly ICommandRegistry _commandRegistry;
    private readonly ICommandInvoker _invoker;

    public CommandInvokerTests()
    {
        IServiceProvider serviceProviderSub = Substitute.For<IServiceProvider>();
        serviceProviderSub.GetService(Arg.Any<Type>()).Returns(callInfo =>
        {
            CommandRegistration? commandRegistration = _commandRegistry?.GetCommands().FirstOrDefault(x => x.Type == callInfo[0] as Type);
            return commandRegistration == null
                ? null
                : Activator.CreateInstance(commandRegistration.Type);
        });

        _commandRegistry = new CommandRegistry(
            Substitute.For<ILogger<CommandRegistry>>(),
            serviceProviderSub);

        _invoker = new CommandInvoker(
            _commandRegistry,
            Substitute.For<ILogger<CommandInvoker>>());
    }

    [Fact]
    public async Task TryInvokeCommandAsync_NameOfRegisteredCommand_InvokedSuccessfully()
    {
        // Arrange
        _commandRegistry.RegisterCommand(typeof(CommandWithoutParameters), true);

        // Act
        bool commandInvokedSuccessfully = await _invoker.TryInvokeCommandAsync($"{nameof(CommandWithoutParameters)}".AsMemory());

        // Assert
        Assert.True(commandInvokedSuccessfully);
    }

    [Fact]
    public async Task TryInvokeCommandAsync_NameOfUnregisteredCommand_InvokedSuccessfully()
    {
        // Act
        bool commandInvokedSuccessfully = await _invoker.TryInvokeCommandAsync($"{nameof(CommandWithoutParameters)}".AsMemory());

        // Assert
        Assert.False(commandInvokedSuccessfully);
    }

    [Fact]
    public async Task TryInvokeCommandAsync_IncorrectCommandName_InvokedSuccessfully()
    {
        // Arrange
        _commandRegistry.RegisterCommand(typeof(CommandWithoutParameters), true);

        // Act
        bool commandInvokedSuccessfully = await _invoker.TryInvokeCommandAsync("NotARegisteredCommandName".AsMemory());

        // Assert
        Assert.False(commandInvokedSuccessfully);
    }

    [Command(nameof(CommandWithoutParameters))]
    private class CommandWithoutParameters : ICommand
    {
        public Task InvokeAsync() => Task.CompletedTask;
    }
}
