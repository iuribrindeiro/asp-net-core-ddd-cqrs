using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bus.Events;
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

        public Task SaveEventAsync(IntegrationEvent @event)
        {
            var eventLogEntry = new IntegrationEventLogEntry(@event);
            _integrationEventLogContext.IntegrationEventsLogs.Add(eventLogEntry);
            return _integrationEventLogContext.SaveChangesAsync();
        }


        public Task MarkEventAsPublishedAsync(IntegrationEvent @event)
        {
            var eventLogEntry = _integrationEventLogContext.IntegrationEventsLogs.Single(ie => ie.EventId == @event.Id);
            eventLogEntry.MarkAsPublished();
            return _integrationEventLogContext.SaveChangesAsync();
        }
    }
}
