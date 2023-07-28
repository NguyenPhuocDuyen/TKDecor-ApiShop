using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_TKDecor.Core.Response
{
    public class ErrorContent
    {
        //common
        //public static string Error = "Đã xảy ra lỗi, hãy thử lại sau!";

        public static string Data = "Xung đột dữ liệu hãy thử lại sau!";
        public static string NotMatchId = "ID không khớp!";

        //not found
        public static string RoleNotFound = "Không tìm thấy vai trò!";
        public static string UserNotFound = "Không tìm thấy người dùng!";
        public static string ProductNotFound = "Không tìm thấy sản phẩm!";
        public static string ArticleNotFound = "Không tìm thấy bài viết!";
        public static string AddressNotFound = "Không tìm thấy địa chỉ!";
        public static string CategoryNotFound = "Không tìm thấy danh mục!";
        public static string CouponNotFound = "Không tìm thấy mã giảm giá!";
        public static string CouponTypeNotFound = "Không tìm thấy loại mã giảm giá!";
        public static string CartNotFound = "Không tìm thấy item trong giỏ hàng!";
        public static string OrderNotFound = "Không tìm thấy đơn hàng!";
        public static string ProductReportNotFound = "Không tìm thấy báo cáo sản phẩm!";
        public static string ProductReviewNotFound = "Không tìm thấy đánh giá sản phẩm!";
        public static string InteractionNotFound = "Không tìm thấy tương tác!";
        public static string Model3DNotFound = "không tìm thấy model 3d!";
        public static string ReportProductReviewNotFound = "Không tìm thấy báo cáo đánh giá sản phẩm!";
        public static string OrderStatusNotFound = "Không tìm thấy trạng thái đơn hàng!";
        public static string ReportStatusNotFound = "Không tìm thấy trạng thái báo cáo!";
        
        public static string OrderStatusUnable = "Không thể cập nhật trạng thái đơn hàng!";
        public static string GenderNotFound = "Không tìm thấy giới tính!";

        public static string AccountIncorrect = "Email hoặc mật khẩu không đúng!";
    }
}
