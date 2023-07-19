using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ReportProductReview
{
    public class ReportProductReviewUpdateDto
    {
        public Guid ReportProductReviewId { get; set; }

        [RegularExpression($"^(Accept|Reject)$")]
        public string ReportStatus { get; set; } = null!;

        //public string Reason { get; set; } = null!;
    }
}
