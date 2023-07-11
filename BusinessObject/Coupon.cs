﻿using Utility.SD;

namespace BusinessObject;

public partial class Coupon : BaseEntity
{
    public Guid CouponId { get; set; }

    public CouponType CouponType { get; set; }

    public string Code { get; set; } = null!;

    public decimal Value { get; set; }

    public decimal MaxValue { get; set; }

    public int RemainingUsageCount { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);

    public bool IsActive { get; set; } = false;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
