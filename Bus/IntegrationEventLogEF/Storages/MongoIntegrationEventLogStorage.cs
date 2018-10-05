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

        public IntegrationEventLog Find(Guid id) => _integrationEventLogs.AsQueryable().FirstOrDefault(ie => ie.EventId == id);

        public void Add(IntegrationEventLog integrationEventLog) => _integrationEventLogs.InsertOne(integrationEventLog);
    }
}