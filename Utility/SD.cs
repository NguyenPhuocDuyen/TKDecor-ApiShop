using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class SD
    {
        // signal R notification
        public const string NewNotification = "NewNotification";

        // role
        public const string RoleAdmin = "Quản trị viên";
        public const string RoleCustomer = "Khách hàng";

        // coupon type
        public const string CouponByPercent = "Giảm theo phần trăm";
        public const string CouponByValue = "Giảm theo giá tiền";

        // gender
        public const string GenderMale = "Nam";
        public const string GenderFemale = "Nữ";
        public const string GenderOther = "Khác";

        // interaction
        public const string InteractionLike = "Thích";
        public const string InteractionDislike = "Không thích";
        public const string InteractionNormal = "Bình thường";

        // order
        public const string OrderOrdered = "Đã đặt đơn hàng";
        public const string OrderDelivering = "Đang giao đơn hàng";
        public const string OrderReceived = "Đã nhận đơn hàng";
        public const string OrderCanceled = "Đã huỷ đơn hàng";

        // report status
        public const string ReportPending = "Chưa xử lý";
        public const string ReportAccept = "Đã chấp nhận";
        public const string ReportReject = "Đã từ chối";
    }
}
