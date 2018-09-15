using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bus.Events;

namespace Bus.EventStore
{
    public interface IIntegrationEventStoreRepository
    {
        Task SaveAsync(IntegrationEvent @event);
    }
}
