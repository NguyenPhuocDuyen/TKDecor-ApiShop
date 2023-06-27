using DataAccess.StatusContent;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ReportProductReview
{
    public class ReportProductReviewUpdateDto
    {
        public int ReportProductReviewId { get; set; }

        [RegularExpression($"^({ReportStatusContent.Accept}|{ReportStatusContent.Reject})$")]
        public string ReportStatus { get; set; } = null!;

        public string Reason { get; set; } = null!;
    }
}
