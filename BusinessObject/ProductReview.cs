namespace BusinessObject;

public partial class ProductReview : BaseEntity
{
    public Guid ProductReviewId { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public int Rate { get; set; }

    public string Description { get; set; } = null!;
     
    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<ReportProductReview> ReportProductReviews { get; set; } = new List<ReportProductReview>();

    public virtual ICollection<ProductReviewInteraction> ProductReviewInteractions { get; set; } = new List<ProductReviewInteraction>();
}
