using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Other
{
    public class OrderStatusContent
    {
        public const string Ordered = "Đặt đơn hàng";
        public const string DeliveringOrders = "Đang giao hàng";
        public const string OrderReceived = "Đã nhận hàng";
        public const string OrderRefund = "Hoàn trả đơn hàng";
        public const string OrderCanceled = "Đã huỷ đơn hàng";
    }
}
