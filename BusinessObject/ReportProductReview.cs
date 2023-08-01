namespace BusinessObject;

public partial class ReportProductReview : BaseEntity
{
    public Guid ReportProductReviewId { get; set; }

    public Guid UserReportId { get; set; }

    public Guid ProductReviewReportedId { get; set; }

    public string ReportStatus { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public virtual ProductReview ProductReviewReported { get; set; } = null!;

    public virtual User UserReport { get; set; } = null!;
}
