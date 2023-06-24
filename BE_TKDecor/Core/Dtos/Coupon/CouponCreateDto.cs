using BusinessObject;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Coupon
{
    public class CouponCreateDto
    {
        public int CouponTypeId { get; set; }

        [MaxLength(255)]
        public string Code { get; set; } = null!;

        [Range(0, 9999999)]
        public decimal Value { get; set; }

        [Range(1, int.MaxValue)]
        public int RemainingUsageCount { get; set; }

        public DateTime? StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
    }
}
