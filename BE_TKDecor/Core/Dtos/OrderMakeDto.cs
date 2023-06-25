using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos
{
    public class OrderMakeDto
    {
        public List<int> ListCartIdSelect { get; set; } = new List<int>();
        public int AddressId { get; set; }
        [MaxLength(255)]
        public string? CodeCoupon { get; set; }
    }
}
