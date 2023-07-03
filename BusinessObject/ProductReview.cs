using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductReview : BaseEntity
{
    public long ProductReviewId { get; set; }

    public long UserId { get; set; }

    public long ProductId { get; set; }

    public int Rate { get; set; }

    public string Description { get; set; } = null!;
     
    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<ReportProductReview> ReportProductReviews { get; set; } = new List<ReportProductReview>();

    public virtual ICollection<ProductReviewInteraction> ProductReviewInteractions { get; set; } = new List<ProductReviewInteraction>();
}
