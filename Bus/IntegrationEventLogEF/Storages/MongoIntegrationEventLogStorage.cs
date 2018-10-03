using System;
using System.Collections.Generic;
using System.Linq;
using Bus.IntegrationEventLogEF.Exceptions;
using Bus.IntegrationEventLogEF.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bus.IntegrationEventLogEF.Storages
{
    public class MongoIntegrationEventLogStorage : IIntegrationEventLogStorage
    {
        private IMongoCollection<IntegrationEventLog> _integrationEventLogs;

        public MongoIntegrationEventLogStorage(IMongoDatabase mongoDatabase)
        {
            _integrationEventLogs = mongoDatabase.GetCollection<IntegrationEventLog>(typeof(IntegrationEventLog).Name);
        }

        public IntegrationEventLog Find(Guid id)
        {
            return _integrationEventLogs.AsQueryable().FirstOrDefault(ie => ie.EventId == id);
        }

        public void Add(IntegrationEventLog integrationEventLog)
        {
            var integrationEventInDatabase = Find(integrationEventLog.EventId);
            
            if (integrationEventInDatabase != null)
                Update(integrationEventLog);
            else
                _integrationEventLogs.InsertOne(integrationEventLog);
        }

        public void Update(IntegrationEventLog integrationEventLog)
        {
            var integrationEventInDatabase = Find(integrationEventLog.EventId);
            
            if (integrationEventInDatabase == null)
                throw new IntegrationEventWithIdDoesNotExistsException(integrationEventLog.EventId);

            _integrationEventLogs.UpdateOne(
                filter: new BsonDocument("EventId", integrationEventLog.EventId), 
                update: new ObjectUpdateDefinition<IntegrationEventLog>(integrationEventLog)
            );
        }

        public void Remove(IntegrationEventLog integrationEventLog)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}