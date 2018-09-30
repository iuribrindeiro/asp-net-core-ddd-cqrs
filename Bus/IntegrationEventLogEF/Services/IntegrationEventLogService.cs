using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bus.Events;
using Bus.IntegrationEventLogEF.Exceptions;
using Bus.IntegrationEventLogEF.Models;
using Microsoft.EntityFrameworkCore;

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
            var eventLogEntry = new IntegrationEventLogEntry(@event);
            _integrationEventLogContext.IntegrationEventsLogs.Add(eventLogEntry);
        }

        public async Task MarkEventAsPublishedAsync(IntegrationEvent @event)
        {
            var eventLogEntry = _integrationEventLogContext.IntegrationEventsLogs.Single(ie => ie.EventId == @event.Id);
            try
            {
                eventLogEntry.MarkAsPublished();
                await _integrationEventLogContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new FailToMarkEventAsPublishedException(eventLogEntry, exception);
            }
        }
    }
}
