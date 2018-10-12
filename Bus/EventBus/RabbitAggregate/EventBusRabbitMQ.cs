using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Bus.EventBus.Exceptions;
using Bus.EventHandler;
using Bus.EventManager;
using Bus.Events;
using Bus.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Bus.EventBus.RabbitAggregate
{
    public class EventBusRabbitMQ : IEventBus
    {
        const string BROKER_NAME = "test_evnt_bus_core_event_bus";

        private const string AUTOFAC_SCOPE_NAME = "eshop_event_bus";
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IIntegrationEventManager _eventManager;
        private readonly ILifetimeScope _lifeScope;
        private readonly int _retryCount;
        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            IIntegrationEventManager eventManager, ILifetimeScope lifeScope, string queueName, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _lifeScope = lifeScope ?? throw new ArgumentNullException(nameof(persistentConnection));
            _queueName = queueName;
            _retryCount = retryCount;
            _consumerChannel = CreateConsumerChannel();
            _eventManager.OnEventRemoved += EventManager_OnEventRemoved;
        }

        private void EventManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.Connect();

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName, exchange: BROKER_NAME, routingKey: eventName);

                if (_eventManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        public void Publish(IntegrationEvent integrationEvent)
        {
            SendToRabbit(integrationEvent);
        }

        public void Subscribe<TEvent, TEventHandler>() where TEvent : IntegrationEvent where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            if (_eventManager.HasSubscriptionsForEvent<TEvent>())
                return;

            if (!_persistentConnection.IsConnected)
                _persistentConnection.Connect();

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = _eventManager.GetEventName<TEvent>();

                channel.QueueBind(queue: _queueName, exchange: BROKER_NAME, routingKey: eventName);
                _eventManager.AddSubscription<TEvent, TEventHandler>();
            }
        }

        public void SubscribeDynamic<TEventHandler>(string eventName) where TEventHandler : IDynamicIntegrationEventHandler
        {
            if (_eventManager.HasSubscriptionsForEvent(eventName))
                return;

            if (!_persistentConnection.IsConnected)
                _persistentConnection.Connect();

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueBind(queue: _queueName, exchange: BROKER_NAME, routingKey: eventName);
                _eventManager.AddDynamicSubscription<TEventHandler>(eventName);
            }
        }

        public void Unsubscribe<TEvent, TEventHandler>() where TEvent : IntegrationEvent where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            _eventManager.RemoveSubscription<TEvent, TEventHandler>();
        }

        public void Unsubscribe<TEventHandler>(string eventName) where TEventHandler : IDynamicIntegrationEventHandler
        {
            _eventManager.RemoveDynamicSubscription<TEventHandler>(eventName);
        }

        private void SendToRabbit(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.Connect();

            var eventName = @event.GetType().Name;
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            var eventSent = false;
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        _logger.LogWarning(
                            $"Fail to publish event: {eventName}, id: {@event.Id} \n error: {ex.ToString()}");
                    });

            policy.Execute(() =>
            {
                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

                    var props = channel.CreateBasicProperties();
                    props.DeliveryMode = 2;

                    channel.BasicPublish(exchange: BROKER_NAME,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: props,
                        body: body);

                    eventSent = true;
                }
            });

            if (!eventSent)
            {
                var failToSentEventException = new FailToSendEventToRabbitException(eventName, message);
                _logger.LogCritical(failToSentEventException.ToString());
                throw failToSentEventException;
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.Connect();

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(BROKER_NAME, "direct");
            channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                var policy = Policy.Handle<FailToProcessEventException>()
                    .WaitAndRetry(13, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning($"{ex} \n DeliveryTag: {ea.DeliveryTag}");
                    });

                bool eventProcessed = false;
                await policy.Execute(async () =>
                {
                    try
                    {
                        await ProcessEvent(eventName, message);
                        eventProcessed = true;
                    }
                    catch (FailToProcessEventException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical($"Fail to process event: {ex} \n DeliveryTag: {ea.DeliveryTag}");
                    }
                });

                if (!eventProcessed)
                    _logger.LogError($"Event: {eventName} could not be processed. See previous log for more info. \n data: {message} \n DeliveryTag: {ea.DeliveryTag}");

                channel.BasicAck(ea.DeliveryTag, multiple:false);
            };

            channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer
            );

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_eventManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _lifeScope.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _eventManager.GetSubscriptionsForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        try
                        {
                            if (subscription.IsDynamic)
                            {
                                var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                                await handler.Handle(JObject.Parse(message));
                            }
                            else
                            {
                                var eventType = _eventManager.GetEventTypeByName(eventName);
                                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                                var handler = scope.ResolveOptional(subscription.HandlerType);
                                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new FailToProcessEventException(
                                eventName: eventName,
                                message: message,
                                innerException: ex
                            );
                        }
                    }
                }
            }
        }
    }
}
