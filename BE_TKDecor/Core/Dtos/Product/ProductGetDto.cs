using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Product
{
    public class ProductGetDto
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public int? Product3DModelId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Slug { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDelete { get; set; } = false;

        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
