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

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            DoAddSubscription(typeof(TH), GetEventName<T>(), false);
            _eventTypes.Add(typeof(T));
        }

        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler => DoAddSubscription(typeof(TH), eventName, true);

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var subscriptionToRemove = FindSubscriptionToRemove<T, TH>();
            DoRemoveHandler(GetEventName<T>(), subscriptionToRemove);
        }

        public void RemoveDynamicSubscription<TH>(string eventName) 
            where TH : IDynamicIntegrationEventHandler
        {
            var subscriptionToRemove = FindDynamicSubscriptionToRemove<TH>(eventName);
            DoRemoveHandler(eventName, subscriptionToRemove);
        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent => _subscriptions.ContainsKey(GetEventName<T>());

        public bool HasSubscriptionsForEvent(string eventName) => _subscriptions.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName) => _eventTypes.FirstOrDefault(e => e.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetSubscriptionsForEvent<T>() where T : IntegrationEvent => _subscriptions[GetEventName<T>()];

        public IEnumerable<SubscriptionInfo> GetSubscriptionsForEvent(string eventName) => _subscriptions[eventName];

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

        private SubscriptionInfo FindSubscriptionToRemove<T, TH>() 
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            return DoFindSubscriptionToRemove(GetEventName<T>(), typeof(TH));
        }

        private SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            return DoFindSubscriptionToRemove(eventName, typeof(TH));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType) 
        {
            if (!HasSubscriptionsForEvent(eventName))
                return null;

            return _subscriptions[eventName].FirstOrDefault(s => s.HandlerType == handlerType);
        }

        private void DoRemoveHandler(string eventName, SubscriptionInfo subscription) 
        {
            if (subscription != null) 
            {
                _subscriptions[eventName].Remove(subscription);
                if (!_subscriptions[eventName].Any())
                {
                    _subscriptions.Remove(eventName);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                        _eventTypes.Remove(eventType);

                    RaiseOnEventRemoved(eventName);
                }
            }
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            OnEventRemoved?.Invoke(this, eventName);
        }
    }
}