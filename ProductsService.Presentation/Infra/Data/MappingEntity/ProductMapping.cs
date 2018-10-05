using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsService.Presentation.Models;

namespace ProductsService.Presentation.Infra.Data.MappingEntity
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Description);
            builder.Property(p => p.Price);
            builder.OwnsOne(p => p.Discount, d =>
            {
                d.Property(dic => dic.Amount).IsRequired(false);
                d.Property(dic => dic.IsPercentage).IsRequired(false);
            });
            builder.Ignore(p => p.PriceWithDiscount);
            builder.Ignore(p => p.HasDiscount);
            builder.Property(p => p.InStock);
        }
    }
}
