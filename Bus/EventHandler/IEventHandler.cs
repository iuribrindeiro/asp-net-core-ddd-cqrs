using System.Threading.Tasks;
using Bus.Events;

namespace Bus.EventHandler
{
    public interface IEventHandler<in TEvent> where TEvent : AbstractEvent
    {
        Task Handle(TEvent @event);
    }

    public interface IDynamicEventHandler
    {
        Task Handle(dynamic @event);
    }
}