﻿using System;
using System.Collections.Generic;
using System.Text;
using Bus.EventHandler;
using Bus.Events;

namespace Bus.EventManager
{
    public interface IIntegrationEventManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetSubscriptionsForEvent<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetSubscriptionsForEvent(string eventName);
        string GetEventName<T>();
    }
}
