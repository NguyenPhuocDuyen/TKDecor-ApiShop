using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductImage
{
    public int ProductImageId { get; set; }

    public int ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsDelete { get; set; }

    public virtual Product ProductImageNavigation { get; set; } = null!;
}
