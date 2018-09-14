using System;

namespace Bus.EventBus.Exceptions
{
    public class FailPublishingMessageException : Exception
    {
        public string EventName { get; }
        public string EventDataJson { get; }
        
        public FailPublishingMessageException(string eventName, string eventDataJson) : base($"Fail to publish event {eventName} \n data: {eventDataJson}")
        {
            EventName = eventName;
            EventDataJson = eventDataJson;
        }
    }
}