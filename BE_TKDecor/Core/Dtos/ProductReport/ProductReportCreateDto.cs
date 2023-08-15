using BusinessObject;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ProductReport
{
    public class ProductReportCreateDto
    {
        public Guid ProductReportedId { get; set; }

        [MinLength(5)]
        [MaxLength(255)]
        public string Reason { get; set; } = null!;
    }
}
