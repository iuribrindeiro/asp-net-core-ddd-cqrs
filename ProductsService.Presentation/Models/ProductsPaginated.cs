using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService.Presentation.Models
{
    public class ProductsPaginated : List<Product>
    {
        public ProductsPaginated(List<Product> items, long total, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            AddRange(items);
        }

        public int TotalPages { get; }
        public int PageIndex { get; }
        public bool HasPreviousPage => (PageIndex > 1);
        public bool HasNextPage => (PageIndex < TotalPages);
    }
}
