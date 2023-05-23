using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int OrderStatusId { get; set; }

    public int? CouponId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Coupon? Coupon { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
