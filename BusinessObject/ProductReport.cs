using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class ProductReport : BaseEntity
{
    public long ProductReportId { get; set; }

    public long UserReportId { get; set; }

    public long ProductReportedId { get; set; }

    public long ReportStatusId { get; set; }

    public string Reason { get; set; } = null!;

    public virtual Product ProductReported { get; set; } = null!;

    public virtual ReportStatus ReportStatus { get; set; } = null!;

    public virtual User UserReport { get; set; } = null!;
}
