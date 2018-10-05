using Bus.Events;
using ProductsService.Presentation.Models;

namespace ProductsService.Presentation.Infra.IntegrationEvents
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
