using System;
using System.Collections.Generic;
using System.Linq;
using Bus.EventHandler;
using Bus.Events;

namespace Bus.EventManager
{
    public class IntegrationEventManager : IIntegrationEventManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _subscriptions;
        private readonly List<Type> _eventTypes;
        public event EventHandler<string> OnEventRemoved;

        public IntegrationEventManager()
        {
            _subscriptions = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => !_subscriptions.Keys.Any();
        public void Clear() => _subscriptions.Clear();

        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            DoAddSubscription(typeof(TH), eventName, true);
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            DoAddSubscription(typeof(TH), GetEventName<T>(), false);
            _eventTypes.Add(typeof(T));
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            throw new NotImplementedException();
        }

        public Type GetEventTypeByName(string eventName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        {
            throw new NotImplementedException();
        }

        public string GetEventName<T>()
        {
            return typeof(T).Name;
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionsForEvent(eventName))
                _subscriptions.Add(eventName, new List<SubscriptionInfo>());

            if (_subscriptions[eventName].Any(s => s.HandlerType == handlerType))
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));

            if (isDynamic)
                _subscriptions[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
            else
                _subscriptions[eventName].Add(SubscriptionInfo.Typed(handlerType));
        }
    }
}