namespace BusinessObject;

public partial class Coupon : BaseEntity
{
    public long CouponId { get; set; }

    public long CouponTypeId { get; set; }

    public string Code { get; set; } = null!;

    public decimal Value { get; set; }

    public int RemainingUsageCount { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);

    public bool IsActive { get; set; } = false;

    public virtual CouponType CouponType { get; set; } = null!;
}
