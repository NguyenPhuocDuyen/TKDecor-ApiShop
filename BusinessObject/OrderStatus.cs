using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class OrderStatus
{
    public Guid OrderStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
