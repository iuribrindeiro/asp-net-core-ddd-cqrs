using System.Threading.Tasks;
using Bus.Events;

namespace Bus.EventHandler
{
    public interface IIntegrationEventHandler<in TEvent> where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event);
    }

    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic @event);
    }
}