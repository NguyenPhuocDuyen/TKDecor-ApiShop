using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductReviewInteraction
{
    public int ProductReviewInteractionId { get; set; }

    public int UserId { get; set; }

    public int ProductReviewId { get; set; }

    public int ProductInteractionStatusId { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;

    public virtual ProductReview ProductReview { get; set; } = null!;

    public virtual ProductReviewInteractionStatus ProductInteractionStatus { get; set; } = null!;
}
