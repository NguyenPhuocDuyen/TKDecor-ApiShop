using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Order
{
    public class OrderUpdateStatusDto
    {
        public Guid OrderId { get; set; }

        [RegularExpression($"^(Delivering|Received|Refund|Canceled)$")]
        public string OrderStatus { get; set; } = null!;
    }
}
