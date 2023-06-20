using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Coupon
{
    public class CouponUpdateDto
    {
        public int CouponId { get; set; }

        public int CouponTypeId { get; set; }

        //public string Code { get; set; } = null!;

        public decimal Value { get; set; }

        public int RemainingUsageCount { get; set; }

        public DateTime? StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
    }
}
