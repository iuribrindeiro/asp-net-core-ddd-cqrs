using System;
using Autofac;
using Bus.EventHandler;
using Bus.Events;
using Bus.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Bus.EventBus.RabbitAggregate
{
    public class EventBusRabbitMQ : IEventBus
    {
        const string BROKER_NAME = "eshop_event_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autofac;
        private readonly string AUTOFAC_SCOPE_NAME = "eshop_event_bus";
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private string _queueName;

        public void Publish(IntegrationEvent integrationEvent)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEvent, TEventHandler>() where TEvent : IntegrationEvent where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void SubscribeDynamic<TEventHandler>(string eventName) where TEventHandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent, TEventHandler>() where TEvent : IntegrationEvent where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEventHandler>(string eventName) where TEventHandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }
    }
}
