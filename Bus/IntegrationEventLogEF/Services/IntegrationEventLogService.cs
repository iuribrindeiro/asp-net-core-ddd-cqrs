using System;
using System.Threading.Tasks;
using Bus.Events;
using Bus.IntegrationEventLogEF.Exceptions;
using Bus.IntegrationEventLogEF.Models;

namespace Bus.IntegrationEventLogEF.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext _integrationEventLogContext;

        public IntegrationEventLogService(IntegrationEventLogContext integrationEventLogContext)
        {
            _integrationEventLogContext = integrationEventLogContext ??
                                          throw new ArgumentNullException(nameof(integrationEventLogContext));
        }

        public void Save(IntegrationEvent @event)
        {
            var eventLogEntry = new IntegrationEventLog(@event);
            _integrationEventLogContext.IntegrationEventsLogs.Add(eventLogEntry);
        }

        public async Task MarkEventAsPublishedAsync(IntegrationEvent @event)
        {
            var eventLog = GetValidEventById(@event.Id);

            try
            {
                eventLog.MarkAsPublished();
                await _integrationEventLogContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new FailToChangeEventStateException(eventLog, exception);
            }
        }

        public async Task MarkEventAsFailToPublishAsync(IntegrationEvent @event)
        {
            var eventLog = GetValidEventById(@event.Id);

            try
            {
                eventLog.MarkAsPublishedFailed();
                await _integrationEventLogContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new FailToChangeEventStateException(eventLog, exception);
            }
        }

        public IntegrationEventLog Find(Guid id)
        {
            return _integrationEventLogContext.IntegrationEventsLogs.Find(id);
        }

        private IntegrationEventLog GetValidEventById(Guid eventId)
        {
            var eventLog = Find(eventId);
            if (eventLog == null) throw new IntegrationEventWithIdDoesNotExistsException(eventId);

            return eventLog;
        }
    }
}