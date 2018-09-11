using Bus.EventHandler;
using Bus.Events;

namespace Bus.Interfaces
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent integrationEvent);

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicIntegrationEventHandler;

        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void Unsubscribe<TEventHandler>(string eventName) 
            where TEventHandler : IDynamicIntegrationEventHandler;
    }
}