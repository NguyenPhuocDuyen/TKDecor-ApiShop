namespace BusinessObject;

public partial class CouponType
{
    public long CouponTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}
