using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bus.Events;

namespace ProductsService.Presentation.Services
{
    public interface IIntegrationContextEventService
    {
        Task SaveApplicationContextAndEventStoreContextChangesAsync();
        Task PublishEvent(IntegrationEvent @event);
    }
}
