using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Coupon
{
    public class CouponGetDto
    {
        public Guid CouponId { get; set; }

        public string CouponType { get; set; } = null!;

        public string Code { get; set; } = null!;

        public decimal Value { get; set; }

        public decimal MaxValue { get; set; }

        public int RemainingUsageCount { get; set; }

        public DateTime? StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsActive { get; set; } = false;
    }
}
