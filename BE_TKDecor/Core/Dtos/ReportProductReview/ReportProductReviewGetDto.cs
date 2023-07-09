namespace BE_TKDecor.Core.Dtos.ReportProductReview
{
    public class ReportProductReviewGetDto
    {
        public Guid ReportProductReviewId { get; set; }

        //public int UserReportId { get; set; }

        public string UserReportName { get; set; } = null!;

        public string UserReportEmail { get; set; } = null!;

        //public int ProductReviewReportedId { get; set; }

        public string ProductReviewReportedDescription { get; set; } = null!;

        //public int ReportStatusId { get; set; }

        public string ReportStatusName { get; set; } = null!;

        public string Reason { get; set; } = null!;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
