using System.Threading.Tasks;
using Bus.Events;

namespace Bus.IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        void SaveEvent(IntegrationEvent @event);
        Task MarkEventAsPublishedAsync(IntegrationEvent @event);
    }
}
