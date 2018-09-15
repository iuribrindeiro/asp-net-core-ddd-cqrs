using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ProductsService.Presentation.Models.ValueObjects;

namespace ProductsService.Presentation.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required]
        public string Description { get; set; }
        
        public Discount Discount { get; set; }

        [Required]
        public decimal Price { get; set; }

        public virtual decimal PriceWithDiscount
        {
            get
            {
                if (Discount == null)
                    return Price;

                return Discount.IsPercentage ? Price - Price * (Discount.Amount / 100) : Discount.Amount;
            }
        }
        public virtual bool HasDiscount => Discount != null;
    }
}
