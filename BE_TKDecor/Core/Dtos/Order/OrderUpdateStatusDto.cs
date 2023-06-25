using DataAccess.StatusContent;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Order
{
    public class OrderUpdateStatusDto
    {
        public int OrderId { get; set; }

        [RegularExpression($"^({OrderStatusContent.Delivering}|{OrderStatusContent.Received}|{OrderStatusContent.Refund}|{OrderStatusContent.Canceled})$")]
        public string OrderStatusName { get; set; } = null!;
    }
}
