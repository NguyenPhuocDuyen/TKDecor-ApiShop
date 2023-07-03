using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductReviewInteraction : BaseEntity
{
    public long ProductReviewInteractionId { get; set; }

    public long UserId { get; set; }

    public long ProductReviewId { get; set; }

    public long ProductInteractionStatusId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ProductReview ProductReview { get; set; } = null!;

    public virtual ProductReviewInteractionStatus ProductInteractionStatus { get; set; } = null!;
}
