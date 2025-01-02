using Archean.Application.Services.Events;
using Archean.Core.Models.Events;
using Archean.Core.Services.Events;

namespace Archean.Tests.Events;

public class EventListenerTests
{
    private IScopedEventBus bus;
    private IGlobalEventBus globalBus;

    public EventListenerTests()
    {
        bus = new EventBus();
        globalBus = new EventBus();
    }

    [Fact]
    public async Task Subscribe_InvokeSubscribedEvent_ExpectedListenerCallback()
    {
        // Setup
        bool hasEventBeenInvoked = false;
        IScopedEventListener listener = new ScopedEventListener(bus, globalBus);

        Action<TestEvent> eventHandle = args => hasEventBeenInvoked = true;
        listener.Subscribe(eventHandle, default);

        // Action
        await bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.True(hasEventBeenInvoked);
    }

    [Fact]
    public async Task Subscribe_InvokeNotSubscribedEvent_ExpectedNoListenerCallback()
    {
        // Setup
        bool hasEventBeenInvoked = false;
        IScopedEventListener listener = new ScopedEventListener(bus, globalBus);

        Action<TestEvent> eventHandle = args => hasEventBeenInvoked = true;

        // Action
        await bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.False(hasEventBeenInvoked);
    }

    [Fact]
    public async Task Subscribe_InvokeUnsubscribedEvent_ExpectedNoListenerCallback()
    {
        // Setup
        bool hasEventBeenInvoked = false;
        IScopedEventListener listener = new ScopedEventListener(bus, globalBus);

        Action<TestEvent> eventHandle = args => hasEventBeenInvoked = true;
        listener.Subscribe(eventHandle, default);
        listener.Unsubscribe(eventHandle);

        // Action
        await bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.False(hasEventBeenInvoked);
    }

    [Fact]
    public async Task Subscribe_PrioritizedSubscriptions_ExpectedCallbackOrder()
    {
        // Setup
        IScopedEventListener listener = new ScopedEventListener(bus, globalBus);

        List<int> numbers = [];
        listener.Subscribe<TestEvent>(_ => numbers.Add(1), (EventPriority)1);
        listener.Subscribe<TestEvent>(_ => numbers.Add(2), (EventPriority)2);
        listener.Subscribe<TestEvent>(_ => numbers.Add(3), (EventPriority)3);

        // Action
        await bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.Equal([3, 2, 1], numbers);
    }

    [Fact]
    public async Task Subscribe_EventCancellation_ExpectedCallbackOrder()
    {
        // Setup
        IScopedEventListener listener = new ScopedEventListener(bus, globalBus);

        List<int> numbers = [];
        listener.Subscribe<TestEvent>(_ => numbers.Add(1), (EventPriority)1);
        listener.Subscribe<TestEvent>(args =>
        {
            numbers.Add(2);
            args.Cancel();
        }, (EventPriority)2);
        listener.Subscribe<TestEvent>(_ => numbers.Add(3), (EventPriority)3);

        // Action
        await bus.InvokeEventAsync(new TestEvent());

        // Assert
        Assert.Equal([3, 2], numbers);
    }

    private class TestEvent : Event
    {
    }
}
