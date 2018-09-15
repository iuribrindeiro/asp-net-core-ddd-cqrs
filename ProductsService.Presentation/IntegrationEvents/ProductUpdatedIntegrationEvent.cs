using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bus.Events;
using ProductsService.Presentation.Models;

namespace ProductsService.Presentation.IntegrationEvents
{
    public class ProductUpdatedIntegrationEvent : IntegrationEvent
    {
        public ProductUpdatedIntegrationEvent(Product product)
        {
            Product = product;
        }

        public Product Product { get; }
    }
}
