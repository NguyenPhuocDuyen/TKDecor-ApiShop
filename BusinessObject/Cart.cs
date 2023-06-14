using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
