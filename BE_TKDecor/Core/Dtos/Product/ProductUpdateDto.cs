using BusinessObject;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Product
{
    public class ProductUpdateDto
    {
        public Guid ProductId { get; set; }

        public Guid CategoryId { get; set; }

        public Guid? Product3DModelId { get; set; }

        [MinLength(2)]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        //[MinLength(50)]
        public string Description { get; set; } = null!;

        [Range(0, 1000000)]
        public int Quantity { get; set; }

        [Range(1, 9999999999)]
        public decimal Price { get; set; }

        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
