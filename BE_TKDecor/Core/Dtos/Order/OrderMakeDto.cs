using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Order
{
    public class OrderMakeDto
    {
        public List<Guid> ListCartIdSelect { get; set; } = new List<Guid>();

        public Guid AddressId { get; set; }

        [MaxLength(50)]
        public string? CodeCoupon { get; set; }

        [MaxLength(255)]
        public string? Note { get; set; }
    }
}
