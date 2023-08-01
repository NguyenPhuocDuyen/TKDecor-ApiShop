using BusinessObject;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.Coupon
{
    public class CouponUpdateDto
    {
        public Guid CouponId { get; set; }

        [RegularExpression($"^({SD.CouponByPercent}|{SD.CouponByValue})$")]
        public string CouponType { get; set; } = null!;

        [Range(0, 9999999)]
        public decimal Value { get; set; }

        [Range(0, 9999999)]
        public decimal MaxValue { get; set; }

        [Range(1, int.MaxValue)]
        public int RemainingUsageCount { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
