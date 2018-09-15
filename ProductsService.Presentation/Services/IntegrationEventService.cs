using System;
using System.Threading.Tasks;
using Bus.Events;
using Bus.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProductsService.Presentation.Infra.ApplicationContext;

namespace ProductsService.Presentation.Services
{
    public class IntegrationEventService : IIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly ProductsApplicationContext _applicationContext;
        private readonly IMongoDatabase _mongoDatabase;

        public IntegrationEventService(IEventBus eventBus, ProductsApplicationContext applicationContext, IMongoDatabase mongoDatabase)
        {
            _eventBus = eventBus;
            _applicationContext = applicationContext;
            _mongoDatabase = mongoDatabase;
        }

        public async Task SaveProductsContextChangesAndSaveSendEvent(IntegrationEvent @event)
        {
            var strategy = _applicationContext.Database.CreateExecutionStrategy();
            var session = await _mongoDatabase.Client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = _applicationContext.Database.BeginTransaction())
                    {
                        await _applicationContext.SaveChangesAsync();
                        await _mongoDatabase.GetCollection<IntegrationEvent>(typeof(IntegrationEvent).Name)
                            .InsertOneAsync(@event);
                        _eventBus.Publish(@event);
                    }
                });
            }
            catch (Exception exception)
            {
                session.AbortTransaction();
                throw;
            }
        }
    }
}
