namespace BusinessObject;

public partial class ProductReviewInteraction : BaseEntity
{
    public Guid ProductReviewInteractionId { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductReviewId { get; set; }

    public string Interaction { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ProductReview ProductReview { get; set; } = null!;
}
