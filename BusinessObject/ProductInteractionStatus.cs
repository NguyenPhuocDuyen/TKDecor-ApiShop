using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductInteractionStatus
{
    public int ProductInteractionStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductInteraction> ProductInteractions { get; set; } = new List<ProductInteraction>();
}
