using Archean.Core.Models.Events;
using Archean.Core.Services.Events;
using Archean.Events.Services;

namespace Archean.Tests.Events;

public class EventListenerTests
{
    private readonly IScopedEventBus _bus;
    private readonly IGlobalEventBus _globalBus;

    public EventListenerTests()
    {
        _bus = new EventBus(null!);
        _globalBus = new EventBus(null!);
    }

    [Fact]
    public async Task Subscribe_InvokeSubscribedEvent_ExpectedListenerCallback()
    {
        // Arrange
        bool hasEventBeenInvoked = false;
        IScopedEventListener listener = new ScopedEventListener(_bus, _globalBus);

        Action<TestEvent> eventHandle = args => hasEventBeenInvoked = true;
        listener.Subscribe(eventHandle);

        // Act
        await _bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.True(hasEventBeenInvoked);
    }

    [Fact]
    public async Task Subscribe_InvokeUnsubscribedEvent_ExpectedNoListenerCallback()
    {
        // Arrange
        bool hasEventBeenInvoked = false;
        IScopedEventListener listener = new ScopedEventListener(_bus, _globalBus);

        Action<TestEvent> eventHandle = args => hasEventBeenInvoked = true;
        listener.Subscribe(eventHandle);
        listener.Unsubscribe(eventHandle);

        // Act
        await _bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.False(hasEventBeenInvoked);
    }

    [Fact]
    public async Task Subscribe_PrioritizedSubscriptions_ExpectedCallbackOrder()
    {
        // Arrange
        IScopedEventListener listener = new ScopedEventListener(_bus, _globalBus);

        List<int> numbers = [];
        listener.Subscribe<TestEvent>(_ => numbers.Add(1), (EventPriority)1);
        listener.Subscribe<TestEvent>(_ => numbers.Add(2), (EventPriority)2);
        listener.Subscribe<TestEvent>(_ => numbers.Add(3), (EventPriority)3);

        // Act
        await _bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.Equal([3, 2, 1], numbers);
    }

    [Fact]
    public async Task Subscribe_EventCancellation_ExpectedCallbackOrder()
    {
        // Arrange
        IScopedEventListener listener = new ScopedEventListener(_bus, _globalBus);

        List<int> numbers = [];
        listener.Subscribe<TestEvent>(_ => numbers.Add(1), (EventPriority)1);
        listener.Subscribe<TestEvent>(args =>
        {
            numbers.Add(2);
            args.Cancel();
        }, (EventPriority)2);
        listener.Subscribe<TestEvent>(_ => numbers.Add(3), (EventPriority)3);

        // Act
        await _bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.Equal([3, 2], numbers);
    }

    private class TestEvent : Event
    {
    }
}
