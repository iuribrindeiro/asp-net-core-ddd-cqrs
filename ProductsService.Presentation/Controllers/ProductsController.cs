using System;
using System.Linq;
using System.Threading.Tasks;
using Bus.IntegrationEventLogEF.Services;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Presentation.Infra.Data.ApplicationContext;
using ProductsService.Presentation.Infra.IntegrationEvents;
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

        public ProductsController(ProductsApplicationContext applicationContext,
            IIntegrationContextEventService integrationContextEventService,
            IIntegrationEventLogService integrationEventLogService)
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
            _integrationEventLogService.Save(productCreatedEvent);
            await _integrationContextEventService.SaveApplicationContextAndEventLogsContextChangesAsync();
            await _integrationContextEventService.PublishEvent(productCreatedEvent);

            return CreatedAtAction(nameof(GetById), new {id = product.Id}, null);
        }

        [HttpGet("{id:guid}")]
        public ActionResult GetById([FromRoute] Guid id)
        {
            var product = _applicationContext.Products.Find(id);

            if (product == null) return NotFound(new {Message = "The product does not exists"});

            return new ObjectResult(product);
        }

        [HttpPut]
        public ActionResult Update([FromBody] Product product)
        {
            var currentProduct = _applicationContext.Products.SingleOrDefault(p => p.Id == product.Id);

            if (currentProduct == null) return NotFound(new {Message = "The product does not exists"});

            var productUpdatedEvent = new ProductUpdatedIntegrationEvent(product);
            _applicationContext.Products.Update(product);
            _integrationEventLogService.Save(productUpdatedEvent);
            _integrationContextEventService.SaveApplicationContextAndEventLogsContextChangesAsync();
            _integrationContextEventService.PublishEvent(productUpdatedEvent);

            return new OkObjectResult(product);
        }
    }
}