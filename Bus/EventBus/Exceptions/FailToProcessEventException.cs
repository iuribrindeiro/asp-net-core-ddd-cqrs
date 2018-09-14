using System;
using System.Collections.Generic;
using System.Text;

namespace Bus.EventBus.Exceptions
{
    public class FailToProcessEventException : Exception
    {
        public string EventName { get; }
        public string Message { get; }

        public FailToProcessEventException(string eventName, string message, Exception innerException) : base($"Fail to process event {eventName}; \n data: {message}", innerException)
        {
            EventName = eventName;
            Message = message;
        }
    }
}
