using System.Threading.Tasks;
using Bus.Events.Base;

namespace Bus.EventHandler
{
    public interface IEventHandler<in TEvent> where TEvent : Event
    {
        Task Handle(TEvent @event);
    }

    public interface IDynamicEventHandler
    {
        Task Handle(dynamic @event);
    }
}