using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService.Presentation.Models.ValueObjects
{
    public class Discount
    {
        public decimal Amount { get; set; }
        public bool IsPercentage { get; set; }
    }
}
