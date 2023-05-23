using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class OrderStatus
{
    public int OrderStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
