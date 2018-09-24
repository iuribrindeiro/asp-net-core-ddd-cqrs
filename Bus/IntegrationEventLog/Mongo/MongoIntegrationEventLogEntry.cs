using Bus.Events;

namespace Bus.IntegrationEventLog.Mongo
{
    public class MongoIntegrationEventLogEntry : AbstractIntegrationEventLogEntry
    {
        public MongoIntegrationEventLogEntry(IntegrationEvent @event) : base(@event){}

        protected override dynamic GetContentEvent(IntegrationEvent @event)
        {
            return @event;
        }
    }
}