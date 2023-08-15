using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ReportProductReview
{
    public class ReportProductReviewCreateDto
    {
        public Guid ProductReviewReportedId { get; set; }

        [MinLength(5)]
        [MaxLength(255)]
        public string Reason { get; set; } = null!;
    }
}
