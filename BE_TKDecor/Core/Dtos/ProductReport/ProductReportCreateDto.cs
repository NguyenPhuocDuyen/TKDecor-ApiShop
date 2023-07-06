using BusinessObject;

namespace BE_TKDecor.Core.Dtos.ProductReport
{
    public class ProductReportCreateDto
    {
        public long ProductReportedId { get; set; }

        public string Reason { get; set; } = null!;
    }
}
