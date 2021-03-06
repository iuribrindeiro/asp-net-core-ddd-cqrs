﻿using System;
using System.Threading.Tasks;
using Bus.EventBus.Exceptions;
using Bus.Events;
using Bus.IntegrationEventLogEF;
using Bus.IntegrationEventLogEF.Exceptions;
using Bus.IntegrationEventLogEF.Services;
using Bus.IntegrationEventLogEF.Utils;
using Bus.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProductsService.Presentation.Infra.Data.ApplicationContext;

namespace ProductsService.Presentation.Services
{
    public class IntegrationContextEventService : IIntegrationContextEventService
    {
        private readonly IEventBus _eventBus;
        private readonly IntegrationEventLogContext _integrationEventLogContext;
        private readonly ProductsApplicationContext _dbContext;
        private readonly IIntegrationEventLogService _integrationEventLogService;

        public IntegrationContextEventService(
            ProductsApplicationContext dbContext,
            IIntegrationEventLogService integrationEventLogService,
            IEventBus eventBus,
            IntegrationEventLogContext integrationEventLogContext
        ) {
            _dbContext = dbContext;
            _integrationEventLogService = integrationEventLogService;
            _eventBus = eventBus;
            _integrationEventLogContext = integrationEventLogContext;
        }

        public async Task SaveApplicationContextAndEventLogsContextChangesAsync()
        {
            var resilientTransaction = ResilientTransaction.Create(_dbContext);
            
            await resilientTransaction.ExecuteAsync(async () =>
            {
                await _dbContext.SaveChangesAsync();
                await _integrationEventLogContext.SaveChangesAsync();
            });
        }

        public async Task PublishEvent(IntegrationEvent @event)
        {
            if (_integrationEventLogService.Find(@event.Id) == null)
                throw new IntegrationEventWithIdDoesNotExistsException(@event.Id);

            try
            {
                _eventBus.Publish(@event);
                await _integrationEventLogService.MarkEventAsPublishedAsync(@event);
            }
            catch (FailToSendEventToRabbitException)
            {
                await _integrationEventLogService.MarkEventAsFailToPublishAsync(@event);
                throw;
            }
        }
    }
}
