using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bus.Events;
using Bus.IntegrationEventLogEF.Exceptions;
using Bus.IntegrationEventLogEF.Models;

namespace Bus.IntegrationEventLogEF.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext _integrationEventLogContext;
        private readonly DbConnection _dbConnection;

        public IntegrationEventLogService(DbConnection dbConnection, IntegrationEventLogContext integrationEventLogContext)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _integrationEventLogContext = integrationEventLogContext ?? throw new ArgumentNullException(nameof(integrationEventLogContext));
        }

        public void SaveEvent(IntegrationEvent @event)
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
            if (eventLog == null)
                throw new IntegrationEventWithIdDoesNotExistsException(eventId);
            
            return eventLog;
        }
    }
}
