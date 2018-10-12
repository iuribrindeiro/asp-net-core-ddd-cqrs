using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService.Presentation.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required]
        public string Description { get; set; }

        public int InStock { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
