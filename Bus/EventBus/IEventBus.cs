using Bus.EventHandler;
using Bus.Events;

namespace Bus.Interfaces
{
    public interface IEventBus
    {
        void Publish(AbstractEvent abstractEvent);

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : AbstractEvent
            where TEventHandler : IEventHandler<TEvent>;

        void SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicEventHandler;

        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : AbstractEvent
            where TEventHandler : IEventHandler<TEvent>;

        void Unsubscribe<TEventHandler>(string eventName) 
            where TEventHandler : IDynamicEventHandler;
    }
}