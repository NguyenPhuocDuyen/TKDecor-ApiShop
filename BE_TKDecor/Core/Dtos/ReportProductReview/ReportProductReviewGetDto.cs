using BusinessObject;

namespace BE_TKDecor.Core.Dtos.ReportProductReview
{
    public class ReportProductReviewGetDto : BaseEntity
    {
        public Guid ReportProductReviewId { get; set; }

        public string UserReportName { get; set; } = null!;

        public string UserReportEmail { get; set; } = null!;

        public string ProductReviewReportedDescription { get; set; } = null!;

        public string ReportStatus { get; set; } = null!;

        public string Reason { get; set; } = null!;
    }
}
