using Archean.Core.Models.Events;
using Archean.Core.Services.Events;
using Archean.Events.Services;

namespace Archean.Tests.Events;

public class EventBusTests
{
    [Fact]
    public async Task InvokeEventAsync_SubscribedEvent_ExpectedEventInvocation()
    {
        // Arrange
        bool hasEventBeenInvoked = false;
        IScopedEventBus bus = new EventBus(null!);
        Action<TestEvent> eventHandle = args => hasEventBeenInvoked = true;
        bus.Subscribe(eventHandle);

        // Act
        await bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.True(hasEventBeenInvoked);
    }

    [Fact]
    public async Task InvokeEventAsync_NotSubscribedEvent_ExpectedNoEventInvocation()
    {
        // Arrange
        bool hasEventBeenInvoked = false;
        IScopedEventBus bus = new EventBus(null!);

        // Act
        await bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.False(hasEventBeenInvoked);
    }

    [Fact]
    public async Task InvokeEventAsync_UnsubscribedEvent_ExpectedNoEventInvocation()
    {
        // Arrange
        bool hasEventBeenInvoked = false;
        IScopedEventBus bus = new EventBus(null!);
        Action<TestEvent> eventHandle = args => hasEventBeenInvoked = true;
        bus.Subscribe(eventHandle);
        bus.Unsubscribe<TestEvent>(eventHandle);

        // Act
        await bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.False(hasEventBeenInvoked);
    }

    private class TestEvent : Event
    {
    }
}
