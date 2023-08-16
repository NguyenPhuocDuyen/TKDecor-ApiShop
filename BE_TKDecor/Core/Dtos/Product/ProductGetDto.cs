using BE_TKDecor.Core.Dtos.Product3DModel;
using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Product
{
    public class ProductGetDto : BaseEntity
    {
        public Guid ProductId { get; set; }

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public double AverageRate { get; set; } = 0;

        public int CountRate { get; set; } = 0;

        public bool IsFavorite { get; set; } = false;

        public List<string> ProductImages { get; set; } = new List<string>();

        public virtual Product3DModelGetDto? Product3DModel { get; set; }
    }
}
