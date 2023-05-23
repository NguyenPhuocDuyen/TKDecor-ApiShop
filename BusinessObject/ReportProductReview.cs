using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ReportProductReview
{
    public int ReportProductReviewId { get; set; }

    public int UserReportId { get; set; }

    public int ProductReviewReportedId { get; set; }

    public int ReportStatusId { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ProductReview ProductReviewReported { get; set; } = null!;

    public virtual ReportStatus ReportStatus { get; set; } = null!;

    public virtual User UserReport { get; set; } = null!;
}
