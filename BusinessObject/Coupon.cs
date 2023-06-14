using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject;

public partial class Coupon
{
    [Key]
    public int CouponId { get; set; }

    public int CouponTypeId { get; set; }

    public string Code { get; set; } = null!;

    public decimal Value { get; set; }

    public int RemainingUsageCount { get; set; }

    public DateTime? StartDate { get; set; } = DateTime.UtcNow;

    public DateTime? EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool? IsActive { get; set; } = false;

    public virtual CouponType CouponType { get; set; } = null!;
}
