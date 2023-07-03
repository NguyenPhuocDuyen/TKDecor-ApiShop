using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Order : BaseEntity
{
    public long OrderId { get; set; }

    public long UserId { get; set; }

    public long OrderStatusId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
