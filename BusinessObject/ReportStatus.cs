using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ReportStatus
{
    public Guid ReportStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductReport> ProductReports { get; set; } = new List<ProductReport>();

    public virtual ICollection<ReportProductReview> ReportProductReviews { get; set; } = new List<ReportProductReview>();
}
