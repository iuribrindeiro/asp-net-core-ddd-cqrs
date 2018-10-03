using System;
using System.Threading.Tasks;
using Bus.Events;
using Bus.IntegrationEventLogEF.Models;

namespace Bus.IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        void SaveEvent(IntegrationEvent @event);
        Task MarkEventAsPublishedAsync(IntegrationEvent @event);
        Task MarkEventAsFailToPublishAsync(IntegrationEvent @event);
        IntegrationEventLog Find(Guid id);
    }
}
