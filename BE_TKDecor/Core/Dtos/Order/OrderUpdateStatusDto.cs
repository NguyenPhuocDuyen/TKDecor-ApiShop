using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.Order
{
    public class OrderUpdateStatusDto
    {
        public Guid OrderId { get; set; }

        [RegularExpression($"^({SD.OrderDelivering}|{SD.OrderReceived}|{SD.OrderCanceled})$")]
        public string OrderStatus { get; set; } = null!;
    }
}
