﻿using Bus.Events;
using ProductsService.Presentation.Models;

namespace ProductsService.Presentation.IntegrationEvents
{
    public class ProductCreatedIntegrationEvent : IntegrationEvent
    {
        public ProductCreatedIntegrationEvent(Product product)
        {
            Product = product;
        }

        public Product Product { get; }
    }
}