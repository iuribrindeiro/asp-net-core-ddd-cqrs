using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Bus.Events;
using MongoDB.Driver;

namespace ProductsService.Presentation.Infra.ApplicationContext
{
    public class EventStoreApplicationContext : IUnitOfWork
    {
        private readonly IMongoDatabase _mongoDatabase;
        
        public IList<IntegrationEvent> IntegrationEvents { get; }

        public EventStoreApplicationContext(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
            IntegrationEvents = new List<IntegrationEvent>();
        }

        public void SaveChanges()
        {
            var session = _mongoDatabase.Client.StartSession();
            session.StartTransaction();
            _mongoDatabase.GetCollection<IntegrationEvent>(typeof(IntegrationEvent).Name).InsertMany(IntegrationEvents);
            session.CommitTransaction();
        }

        public async Task SaveChangesAsync()
        {
            var session = await _mongoDatabase.Client.StartSessionAsync();
            session.StartTransaction();
            await _mongoDatabase.GetCollection<IntegrationEvent>(typeof(IntegrationEvent).Name)
                .InsertManyAsync(IntegrationEvents);
            await session.CommitTransactionAsync();
        }
    }
}