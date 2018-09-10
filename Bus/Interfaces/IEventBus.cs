using Bus.EventHandler;
using Bus.Events.Base;

namespace Bus.Interfaces
{
    public interface IEventBus
    {
         void Publish(Event @event);

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>;

        void SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicEventHandler;

        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>;

        void Unsubscribe<TEventHandler>(string eventName) 
            where TEventHandler : IDynamicEventHandler;
    }
}