using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductInteractionStatus
{
    public int ProductReviewInteractionStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductInteraction> ProductInteractions { get; set; } = new List<ProductInteraction>();
}
