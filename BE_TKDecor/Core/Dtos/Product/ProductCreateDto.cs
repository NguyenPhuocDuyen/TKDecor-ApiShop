using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Product
{
    public class ProductCreateDto
    {
        public int CategoryId { get; set; }

        //public int? Product3DModelId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public List<string> Images { get; set; } = new List<string>();
        //public virtual Product3DModel? Product3DModel { get; set; }
    }
}
