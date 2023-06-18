using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_TKDecor.Core.Dtos.Product
{
    public class ProductDto
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string? Url3dModel { get; set; }

        //public List<ProductImage> ProductImages { get; set; }
    }
}
