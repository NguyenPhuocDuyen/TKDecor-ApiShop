using Utility.SD;

namespace BusinessObject;

public partial class ProductReport : BaseEntity
{
    public Guid ProductReportId { get; set; }

    public Guid UserReportId { get; set; }

    public Guid ProductReportedId { get; set; }

    public ReportStatus ReportStatus { get; set; }

    public string Reason { get; set; } = null!;

    public virtual Product ProductReported { get; set; } = null!;

    public virtual User UserReport { get; set; } = null!;
}
