using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ProductReport
{
    public class ProductReportUpdateDto
    { 
        public Guid ProductReportId { get; set; }

        [RegularExpression($"^(Accept|Reject)$")]
        public string ReportStatus { get; set;} = null!;
    }
}
