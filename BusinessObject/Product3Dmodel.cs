using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Product3DModel : BaseEntity
{
    public long Product3DModelId { get; set; }

    public string VideoUrl { get; set; } = null!;

    public string ModelUrl { get; set; } = null!;

    public string ThumbnailUrl { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
