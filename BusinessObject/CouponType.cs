using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class CouponType
{
    public int CouponTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}
