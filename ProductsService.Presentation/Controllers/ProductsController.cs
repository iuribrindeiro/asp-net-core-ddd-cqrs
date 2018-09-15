using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bus.Events;
using Bus.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProductsService.Presentation.Infra.ApplicationContext;
using ProductsService.Presentation.IntegrationEvents;
using ProductsService.Presentation.Models;

namespace ProductsService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsApplicationContext _applicationContext;
        private readonly IEventBus _eventBus;
        private readonly IMongoDatabase _mongoDatabase;

        public ProductsController(ProductsApplicationContext applicationContext, IEventBus eventBus, IMongoDatabase mongoDatabase)
        {
            _applicationContext = applicationContext;
            _eventBus = eventBus;
            _mongoDatabase = mongoDatabase;
        }

        [HttpPost]
        public ActionResult CreateProduct([FromBody] Product product)
        {
            _applicationContext.Products.Add(product);
            _applicationContext.SaveChanges();

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

            _applicationContext.Products.Update(product);
            var productUpdatedEvent = new ProductUpdatedIntegrationEvent(product);
            _mongoDatabase.GetCollection<IntegrationEvent>(typeof(IntegrationEvent).Name).InsertOne(productUpdatedEvent);
            _eventBus.Publish(productUpdatedEvent);
        }
    
    }
}