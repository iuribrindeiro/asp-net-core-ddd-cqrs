using System.Threading.Tasks;
using Bus.IntegrationEventLogEF.Models;
using Bus.IntegrationEventLogEF.Storages;
using Bus.IntegrationEventLogEF.Storages.Proxy;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bus.IntegrationEventLogEF
{
    public class IntegrationEventLogContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        private readonly InMemoryIntegrationEventLogStorage _inMemoryIntegrationEventsLogs;

        public IIntegrationEventLogStorage IntegrationEventsLogs { get; }

        public IntegrationEventLogContext(IMongoDatabase mongoDatabase,
            IIntegrationEventLogStorageFactory integrationEventLogStorage,
            InMemoryIntegrationEventLogStorage inMemoryIntegrationEventLogStorage)
        {
            _mongoDatabase = mongoDatabase;
            IntegrationEventsLogs = integrationEventLogStorage.GetStorage();
            _inMemoryIntegrationEventsLogs = inMemoryIntegrationEventLogStorage;
        }

        public void SaveChanges()
        {
            var session = _mongoDatabase.Client.StartSession();
            session.StartTransaction();
            var collection =
                _mongoDatabase.GetCollection<IntegrationEventLog>(typeof(IntegrationEventLog).Name);

            foreach (var integrationEventLogEntry in _inMemoryIntegrationEventsLogs.IntegrationEventLogs)
                collection.ReplaceOne(
                    session,
                    new BsonDocument("EventId", integrationEventLogEntry.EventId),
                    options: new UpdateOptions {IsUpsert = true},
                    replacement: integrationEventLogEntry
                );
            session.CommitTransaction();
        }

        public async Task SaveChangesAsync()
        {
            var session = await _mongoDatabase.Client.StartSessionAsync();
            session.StartTransaction();
            var collection =
                _mongoDatabase.GetCollection<IntegrationEventLog>(typeof(IntegrationEventLog).Name);

            foreach (var integrationEventLogEntry in _inMemoryIntegrationEventsLogs.IntegrationEventLogs)
                await collection.ReplaceOneAsync(
                    session,
                    new BsonDocument("EventId", integrationEventLogEntry.EventId),
                    options: new UpdateOptions {IsUpsert = true},
                    replacement: integrationEventLogEntry
                );
            await session.CommitTransactionAsync();
        }
    }
}