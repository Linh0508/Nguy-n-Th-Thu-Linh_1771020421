using System;
using System.Collections.Generic;
using MasterDBank.Events;

namespace HaiChanBank.Events
{
    // In-memory event bus (existing implementation) — giờ implement IEventBus
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            if (!_handlers.ContainsKey(typeof(T)))
                _handlers[typeof(T)] = new List<Delegate>();
            _handlers[typeof(T)].Add(handler);
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                foreach (var handler in _handlers[typeof(T)])
                {
                    try
                    {
                        ((Action<T>)handler)(@event);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[EventBus] handler error: {ex.Message}");
                    }
                }
            }
        }
    }
}