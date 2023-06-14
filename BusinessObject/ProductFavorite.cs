using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductFavorite
{
    public int ProductFavoriteId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public DateTime? Created { get; set; } = DateTime.UtcNow;

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
