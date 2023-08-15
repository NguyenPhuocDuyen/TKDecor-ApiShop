using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.Coupon
{
    public class CouponCreateDto
    {
        [RegularExpression($"^({SD.CouponByPercent}|{SD.CouponByValue})$")]
        public string CouponType { get; set; } = null!;

        [MinLength(5)]
        [MaxLength(50)]
        public string Code { get; set; } = null!;

        [Range(0, 99999999)]
        public decimal Value { get; set; }

        [Range(0, 99999999)]
        public decimal MaxValue { get; set; }

        [Range(1, int.MaxValue)]
        public int RemainingUsageCount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
