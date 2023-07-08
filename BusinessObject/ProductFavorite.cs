using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductFavorite : BaseEntity
{
    public Guid ProductFavoriteId { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
