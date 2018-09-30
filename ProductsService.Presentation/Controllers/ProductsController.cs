using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bus.Events;
using Bus.IntegrationEventLogEF.Services;
using Bus.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProductsService.Presentation.Infra.ApplicationContext;
using ProductsService.Presentation.IntegrationEvents;
using ProductsService.Presentation.Models;
using ProductsService.Presentation.Services;

namespace ProductsService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsApplicationContext _applicationContext;
        private readonly IIntegrationContextEventService _integrationContextEventService;
        private readonly IIntegrationEventLogService _integrationEventLogService;

        public ProductsController(
            ProductsApplicationContext applicationContext, 
            IIntegrationContextEventService integrationContextEventService,
            IIntegrationEventLogService integrationEventLogService
        )
        {
            _applicationContext = applicationContext;
            _integrationContextEventService = integrationContextEventService;
            _integrationEventLogService = integrationEventLogService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] Product product)
        {
            _applicationContext.Products.Add(product);
            var productCreatedEvent = new ProductCreatedIntegrationEvent(product);
            _integrationEventLogService.SaveEvent(productCreatedEvent);
            await _integrationContextEventService.SaveApplicationContextAndEventStoreContextChangesAsync();
            await _integrationContextEventService.PublishEvent(productCreatedEvent);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, null);
        }

        [HttpGet("{guid:id}")]
        public ActionResult GetById([FromRoute]Guid id)
        {
            var product = _applicationContext.Products.Find(id);

            if (product == null)
                return NotFound(new { Message = "The product does not exists" });

            return new ObjectResult(product);
        }

        [HttpPut]
        public ActionResult Update([FromBody] Product product)
        {
            var currentProduct = _applicationContext.Products.SingleOrDefault(p => p.Id == product.Id);

            if (currentProduct == null)
                return NotFound(new { Message = "The product does not exists" });

            var productUpdatedEvent = new ProductUpdatedIntegrationEvent(product);
            _applicationContext.Products.Update(product);
            _integrationEventLogService.SaveEvent(productUpdatedEvent);
            _integrationContextEventService.SaveApplicationContextAndEventStoreContextChangesAsync();
            _integrationContextEventService.PublishEvent(productUpdatedEvent);

            return new OkObjectResult(product);
        }
    
    }
}