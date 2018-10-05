using System;
using System.Collections.Generic;
using System.Linq;
using Bus.IntegrationEventLogEF.Models;

namespace Bus.IntegrationEventLogEF.Storages
{
    public interface IIntegrationEventLogStorage
    {
        IntegrationEventLog Find(Guid id);
        void Add(IntegrationEventLog integrationEventLog);
    }
}