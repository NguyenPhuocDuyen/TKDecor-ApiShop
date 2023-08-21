namespace BusinessObject;

public partial class OrderDetail
{
    public Guid OrderDetailId { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? ProductReviewId { get; set; }

    public int Quantity { get; set; }

    public decimal PaymentPrice { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductReview? ProductReview { get; set; }
}
