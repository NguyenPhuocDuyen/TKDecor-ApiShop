using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductReview
{
    public int ProductReviewId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Rate { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool? IsDelete { get; set; } = false;
     
    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<ReportProductReview> ReportProductReviews { get; set; } = new List<ReportProductReview>();

    public virtual ICollection<ProductReviewInteraction> ProductReviewInteractions { get; set; } = new List<ProductReviewInteraction>();
}
