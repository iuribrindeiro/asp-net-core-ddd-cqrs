using System;
using Bus.EventHandler;
using Bus.Events;
using Bus.Interfaces;

namespace Bus.EventBus.RabbitAggregate
{
    public class EventBusRabbitMQ : IEventBus
    {
        public void Publish(AbstractEvent abstractEvent)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEvent, TEventHandler>() where TEvent : AbstractEvent where TEventHandler : IEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void SubscribeDynamic<TEventHandler>(string eventName) where TEventHandler : IDynamicEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent, TEventHandler>() where TEvent : AbstractEvent where TEventHandler : IEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEventHandler>(string eventName) where TEventHandler : IDynamicEventHandler
        {
            throw new NotImplementedException();
        }
    }
}
