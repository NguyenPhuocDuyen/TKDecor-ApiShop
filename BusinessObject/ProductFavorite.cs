﻿using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductFavorite : BaseEntity
{
    public long ProductFavoriteId { get; set; }

    public long UserId { get; set; }

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
