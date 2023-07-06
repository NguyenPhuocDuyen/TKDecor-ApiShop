namespace BE_TKDecor.Core.Dtos.ReportProductReview
{
    public class ReportProductReviewCreateDto
    {
        public long ProductReviewReportedId { get; set; }

        public string Reason { get; set; } = null!;
    }
}
