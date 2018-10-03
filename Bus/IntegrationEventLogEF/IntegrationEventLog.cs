using System;
using Bus.Events;
using Newtonsoft.Json;

namespace Bus.IntegrationEventLogEF.Models
{
    public class IntegrationEventLog
    {
        private IntegrationEventLog() { }
        
        public IntegrationEventLog(IntegrationEvent @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreatedAt;
            EventTypeName = @event.GetType().FullName;
            Content = @event;
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
        }

        public Guid EventId { get; }
        public string EventTypeName { get; }
        public EventStateEnum State { get; private set; }
        public int TimesSent { get; }
        public DateTime CreationTime { get; }
        public dynamic Content { get; }

        public IntegrationEventLog MarkAsPublished()
        {
            State = EventStateEnum.Published;
            return this;
        }
        
        public IntegrationEventLog MarkAsPublishedFailed()
        {
            State = EventStateEnum.PublishedFailed;
            return this;
        }
    }
}