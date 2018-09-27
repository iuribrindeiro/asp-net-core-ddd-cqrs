using System.Threading.Tasks;
using Bus.Events;

namespace Bus.IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        Task SaveEventAsync(IntegrationEvent @event);
        Task MarkEventAsPublishedAsync(IntegrationEvent @event);
    }
}
