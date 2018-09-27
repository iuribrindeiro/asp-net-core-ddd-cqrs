using System;
using Bus.Events;
using Newtonsoft.Json;

namespace Bus.IntegrationEventLogEF.Models
{
    public class IntegrationEventLogEntry
    {
        private IntegrationEventLogEntry() { }
        
        public IntegrationEventLogEntry(IntegrationEvent @event)
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

        public IntegrationEventLogEntry MarkAsPublished()
        {
            State = EventStateEnum.Published;
            return this;
        }
        
        public IntegrationEventLogEntry MarkAsPublishedFailed()
        {
            State = EventStateEnum.PublishedFailed;
            return this;
        }
    }
}