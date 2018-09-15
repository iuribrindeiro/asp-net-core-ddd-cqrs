using System;
using System.Collections.Generic;
using System.Text;

namespace Bus.EventBus.Exceptions
{
    public class FailToSendEventToRabbitException : Exception
    {
        public string EventName { get; set; }
        public string EventData { get; set; }

        public FailToSendEventToRabbitException(string eventName, string eventData) : base($"Fail to send event: {eventName} to rabbit. \n data: {eventData}")
        {
            EventName = eventName;
            EventData = eventData;
        }
    }
}
