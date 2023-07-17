using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Cart
{
    public class CartGetDto : BaseEntity
    {
        public Guid CartId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }

        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
