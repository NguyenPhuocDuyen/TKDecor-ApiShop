using DataAccess.StatusContent;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ProductReport
{
    public class ProductReportUpdateDto
    { 
        public int ProductReportId { get; set; }

        [RegularExpression($"^({ReportStatusContent.Accept}|{ReportStatusContent.Reject})$")]
        public string ReportStatus { get; set;} = null!;
    }
}
