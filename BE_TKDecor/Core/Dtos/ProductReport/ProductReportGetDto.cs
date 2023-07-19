using BusinessObject;
using Utility.SD;

namespace BE_TKDecor.Core.Dtos.ProductReport
{
    public class ProductReportGetDto : BaseEntity
    {
        public Guid ProductReportId { get; set; }

        public string ProductName { get; set; } = null!;

        public string UserReportEmail { get; set; } = null!;

        public string ReportStatus { get; set; } = null!;

        public string Reason { get; set; } = null!;
    }
}
