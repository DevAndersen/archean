﻿using Archean.Core.Models.Events;

namespace Archean.Core.Services.Events;

public interface IEventListener
{
    public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : Event;

    public void Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event;

    public void Subscribe<TEvent>(Action<TEvent> action, int? priority) where TEvent : Event;

    public void Subscribe<TEvent>(Func<TEvent, Task> action, int? priority) where TEvent : Event;

    public void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : Event;

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : Event;
}
