namespace BE_TKDecor.Core.Dtos.Cart
{
    public class CartGetDto
    {
        public int CartId { get; set; }

        //public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }

        public List<string> ProductImages { get; set; } = new List<string>();

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
