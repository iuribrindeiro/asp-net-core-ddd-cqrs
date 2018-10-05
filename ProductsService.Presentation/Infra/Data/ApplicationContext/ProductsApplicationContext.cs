using Microsoft.EntityFrameworkCore;
using ProductsService.Presentation.Infra.Data.MappingEntity;
using ProductsService.Presentation.Models;

namespace ProductsService.Presentation.Infra.Data.ApplicationContext
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
