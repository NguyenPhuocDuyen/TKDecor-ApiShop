using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Product3DModel
{
    public int Product3DModelId { get; set; }

    public string VideoUrl { get; set; } = null!;

    public string ModelUrl { get; set; } = null!;

    public string ThumbnailUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Product? Product { get; set; }
}
