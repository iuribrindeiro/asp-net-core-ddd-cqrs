using System;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Bus.EventHandler;
using Bus.EventManager;
using Bus.Events;
using Bus.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Bus.EventBus.RabbitAggregate
{
    public class EventBusRabbitMQ : IEventBus
    {
        const string BROKER_NAME = "test_evnt_bus_core_event_bus";

        private const string AUTOFAC_SCOPE_NAME = "eshop_event_bus";
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IIntegrationEventManager _subsManager;
        private readonly ILifetimeScope _autofac;
        private readonly int _retryCount;
        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            IIntegrationEventManager subsManager, ILifetimeScope autofac, IModel consumerChannel,
            string queueName, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _autofac = autofac ?? throw new ArgumentNullException(nameof(persistentConnection));
            _consumerChannel = consumerChannel ?? throw new ArgumentNullException(nameof(persistentConnection));
            _queueName = queueName;
            _retryCount = retryCount;
        }

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

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(BROKER_NAME, "direct");
            channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);

                channel.BasicAck(ea.DeliveryTag, multiple:false);
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {

        }
    }
}
