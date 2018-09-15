using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductsService.Presentation.Infra.MappingEntity;
using ProductsService.Presentation.Models;

namespace ProductsService.Presentation.Infra.ApplicationContext
{
    public class ProductsApplicationContext : DbContext
    {
        public ProductsApplicationContext(DbContextOptions<ProductsApplicationContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modeelBuilder)
        {
            modeelBuilder.ApplyConfiguration(new ProductMapping());
        }
    }
}
