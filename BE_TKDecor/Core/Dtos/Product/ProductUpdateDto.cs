using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Product
{
    public class ProductUpdateDto
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        //public int? Product3DModelId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
