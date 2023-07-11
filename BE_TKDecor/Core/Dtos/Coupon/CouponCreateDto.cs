using BusinessObject;
using System.ComponentModel.DataAnnotations;
using Utility.SD;

namespace BE_TKDecor.Core.Dtos.Coupon
{
    public class CouponCreateDto
    {
        [RegularExpression($"^(ByPercent|ByValue)$")]
        public string CouponType { get; set; } = null!;

        [MaxLength(255)]
        public string Code { get; set; } = null!;

        [Range(0, 9999999)]
        public decimal Value { get; set; }

        [Range(1, int.MaxValue)]
        public int RemainingUsageCount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
