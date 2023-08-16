using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Order
{
    public class OrderGetDto : BaseEntity
    {
        public Guid OrderId { get; set; }

        public string OrderStatus { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string? Note { get; set; }

        public decimal TotalPrice { get; set; }

        public virtual ICollection<OrderDetailGetDto> OrderDetails { get; set; } = new List<OrderDetailGetDto>();
    }

    public class OrderDetailGetDto
    {
        public Guid OrderDetailId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string ProductSlug { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal PaymentPrice { get; set; }

        public bool HasUserReviewed { get; set; } = false;

        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
