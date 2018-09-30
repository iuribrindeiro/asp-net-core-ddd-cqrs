using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bus.Events;
using Bus.IntegrationEventLogEF.Models;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bus.IntegrationEventLogEF
{
    public class IntegrationEventLogContext
    {
        private readonly IMongoDatabase _mongoDatabase;
        private static readonly ProxyGenerator _generator = new ProxyGenerator(new PersistentProxyBuilder());

        public IList<IntegrationEventLogEntry> IntegrationEventsLogs { get; }

        public IntegrationEventLogContext(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
            IntegrationEventsLogs = (IList<IntegrationEventLogEntry>)_generator.CreateInterfaceProxyWithTarget<IList<IntegrationEventLogEntry>>(
                _mongoDatabase.GetCollection<IntegrationEventLogEntry>(typeof(IntegrationEventLogEntry).Name), new IInterceptor[ new Intercept]);
        }

        public void SaveChanges()
        {
            var session = _mongoDatabase.Client.StartSession();
            session.StartTransaction();
            var collection =
                _mongoDatabase.GetCollection<IntegrationEventLogEntry>(typeof(IntegrationEventLogEntry).Name);

            foreach (var integrationEventLogEntry in IntegrationEventsLogs)
            {
                collection.ReplaceOne(
                    session,
                    filter: new BsonDocument("EventId", integrationEventLogEntry.EventId),
                    options: new UpdateOptions() { IsUpsert = true },
                    replacement: integrationEventLogEntry
                );
            }
            session.CommitTransaction();
        }

        public async Task SaveChangesAsync()
        {
            var session = await _mongoDatabase.Client.StartSessionAsync();
            session.StartTransaction();
            var collection =
                _mongoDatabase.GetCollection<IntegrationEventLogEntry>(typeof(IntegrationEventLogEntry).Name);

            foreach (var integrationEventLogEntry in IntegrationEventsLogs)
            {
                await collection.ReplaceOneAsync(
                    session,
                    filter: new BsonDocument("EventId", integrationEventLogEntry.EventId),
                    options: new UpdateOptions() {IsUpsert = true}, 
                    replacement: integrationEventLogEntry
                );
            }
            await session.CommitTransactionAsync();
        }
    }
}
