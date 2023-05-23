using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Coupon
{
    public int CouponId { get; set; }

    public int CouponTypeId { get; set; }

    public string Code { get; set; } = null!;

    public decimal Value { get; set; }

    public int RemainingUsageCount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual CouponType CouponType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
