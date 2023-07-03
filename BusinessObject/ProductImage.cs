using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductImage
{
    public long ProductImageId { get; set; }

    public long ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
