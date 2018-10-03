using System;
using System.Collections.Generic;
using System.Linq;
using Bus.IntegrationEventLogEF.Exceptions;
using Bus.IntegrationEventLogEF.Models;

namespace Bus.IntegrationEventLogEF.Storages
{
    public class InMemoryIntegrationEventLogStorage : IIntegrationEventLogStorage
    {
        public IList<IntegrationEventLog> IntegrationEventLogs { get; }

        public InMemoryIntegrationEventLogStorage()
        {
            IntegrationEventLogs = new List<IntegrationEventLog>();
        }

        public IntegrationEventLog Find(Guid id)
        {
            return IntegrationEventLogs.SingleOrDefault(ie => ie.EventId == id);
        }

        public void Add(IntegrationEventLog integrationEventLog)
        {
            var integrationEventInMemory = Find(integrationEventLog.EventId);
            
            if (integrationEventInMemory != null)
                Update(integrationEventLog);
            else
                IntegrationEventLogs.Add(integrationEventLog);
        }

        public void Update(IntegrationEventLog integrationEventLog)
        {
            var integrationEventInMemory = Find(integrationEventLog.EventId);
            
            if (integrationEventInMemory == null)
                throw new IntegrationEventWithIdDoesNotExistsException(integrationEventLog.EventId);
            
            var index = IntegrationEventLogs.IndexOf(integrationEventInMemory);
            IntegrationEventLogs[index] = integrationEventLog;
        }

        public void Remove(IntegrationEventLog integrationEventLog)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}