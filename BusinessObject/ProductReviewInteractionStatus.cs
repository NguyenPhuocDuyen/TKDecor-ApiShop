using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductReviewInteractionStatus
{
    public int ProductReviewInteractionStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductReviewInteraction> ProductReviewInteractions { get; set; } = new List<ProductReviewInteraction>();
}
