using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Order
{
    public class OrderGetDto
    {
        public int OrderId { get; set; }

        public int OrderStatusId { get; set; }

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<OrderDetailGetDto> OrderDetails { get; set; } = new List<OrderDetailGetDto>();
    }

    public class OrderDetailGetDto
    {
        public int OrderDetailId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal PaymentPrice { get; set; }

        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
