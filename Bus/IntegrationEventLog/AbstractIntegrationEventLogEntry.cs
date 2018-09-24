using System;
using Bus.Events;
using Newtonsoft.Json;

namespace Bus.IntegrationEventLog
{
    public abstract class AbstractIntegrationEventLogEntry
    {
        private AbstractIntegrationEventLogEntry() { }
        
        public AbstractIntegrationEventLogEntry(IntegrationEvent @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreatedAt;
            EventTypeName = @event.GetType().FullName;
            Content = GetContentEvent(@event);
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
        }

        protected abstract dynamic GetContentEvent(IntegrationEvent @event);
        
        public Guid EventId { get; }
        public string EventTypeName { get; }
        public EventStateEnum State { get; private set; }
        public int TimesSent { get; }
        public DateTime CreationTime { get; }
        public dynamic Content { get; }

        public AbstractIntegrationEventLogEntry MarkAsPublished()
        {
            State = EventStateEnum.Published;
            return this;
        }
        
        public AbstractIntegrationEventLogEntry MarkAsPublishedFailed()
        {
            State = EventStateEnum.PublishedFailed;
            return this;
        }
    }
}