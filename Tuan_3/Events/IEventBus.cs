using System;
using MasterDBank.Events;

namespace HaiChanBank.Events
{
    // Trừu tượng hoá EventBus để dễ thay thế concrete implementation (in-memory, RabbitMQ, Kafka...)
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : IEvent;
        void Publish<T>(T @event) where T : IEvent;
    }
}