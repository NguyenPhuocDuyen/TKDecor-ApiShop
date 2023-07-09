using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Order : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public Guid OrderStatusId { get; set; }

    public Guid? CouponId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Note { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Coupon? Coupon { get; set; }
}
