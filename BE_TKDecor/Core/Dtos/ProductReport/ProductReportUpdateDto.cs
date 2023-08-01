using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.ProductReport
{
    public class ProductReportUpdateDto
    { 
        public Guid ProductReportId { get; set; }

        [RegularExpression($"^({SD.ReportAccept}|{SD.ReportReject})$")]
        public string ReportStatus { get; set;} = null!;
    }
}
