using BusinessObject;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Coupon
{
    public class CouponUpdateDto
    {
        public long CouponId { get; set; }

        public long CouponTypeId { get; set; }

        [Range(0, 9999999)]
        public decimal Value { get; set; }

        [Range(1, int.MaxValue)]
        public int RemainingUsageCount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
