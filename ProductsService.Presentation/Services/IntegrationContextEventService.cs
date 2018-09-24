using System;
using System.Threading.Tasks;
using Bus.Events;
using Bus.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProductsService.Presentation.Infra.ApplicationContext;

namespace ProductsService.Presentation.Services
{
    public class IntegrationContextEventService : IIntegrationContextEventService
    {
        private readonly IEventBus _eventBus;
        private readonly DbContext _dbContext;
        private readonly IUnitOfWork _eventStoreUnitOfWork;

        public IntegrationContextEventService(
            DbContext dbContext, 
            IUnitOfWork eventStoreUnitOfWork, 
            IEventBus eventBus)
        {
            _dbContext = dbContext;
            _eventStoreUnitOfWork = eventStoreUnitOfWork;
            _eventBus = eventBus;
        }

        public async Task SaveApplicationContextAndEventStoreContextChangesAsync()
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    await _dbContext.SaveChangesAsync();
                    await _eventStoreUnitOfWork.SaveChangesAsync();
                }
            });
        }

        public void PublishEvent(IntegrationEvent @event)
        {
            _eventBus.Publish(@event);
        }
    }
}
