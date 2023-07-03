using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ReportProductReview : BaseEntity
{
    public long ReportProductReviewId { get; set; }

    public long UserReportId { get; set; }

    public long ProductReviewReportedId { get; set; }

    public long ReportStatusId { get; set; }

    public string Reason { get; set; } = null!;

    public virtual ProductReview ProductReviewReported { get; set; } = null!;

    public virtual ReportStatus ReportStatus { get; set; } = null!;

    public virtual User UserReport { get; set; } = null!;
}
