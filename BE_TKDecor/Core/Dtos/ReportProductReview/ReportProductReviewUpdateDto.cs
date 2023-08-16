using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.ReportProductReview
{
    public class ReportProductReviewUpdateDto
    {
        public Guid ReportProductReviewId { get; set; }

        [RegularExpression($"^({SD.ReportAccept}|{SD.ReportReject})$")]
        public string ReportStatus { get; set; } = null!;
    }
}
