using System;
using System.Collections.Generic;
using System.Linq;
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
            IntegrationEventLogs.Add(integrationEventLog);
        }
    }
}