using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Coupon
{
    public class CouponGetDto : BaseEntity
    {
        public Guid CouponId { get; set; }

        public string CouponType { get; set; } = null!;

        public string Code { get; set; } = null!;

        public decimal Value { get; set; }

        public decimal MaxValue { get; set; }

        public int RemainingUsageCount { get; set; }

        public DateTime StartDate { get; set; } 

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
