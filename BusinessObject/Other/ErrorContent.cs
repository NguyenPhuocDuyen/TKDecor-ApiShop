using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Other
{
    public class ErrorContent
    {
        //error
        public static string Error = "Có lỗi xảy ra thử lại sau!";

        //mail error
        public static string LoginFail = "Email hoặc mật khẩu không chính xác!";
        public static string SendEmail = "Gửi email để xác nhận thất bại!";
        public static string EmailExists = "Email đã tồn tại!";
        public static string NotConfirmEmail = "Email chưa xác nhận!";

        //not found
        public static string NotFound = "Không tìm thấy!";
        public static string NotFoundProduct = "Không tìm thấy sản phẩm trong giỏ hàng!";

        //user error
        public static string UserNotFound = "Không tìm thấy người dùng!";

        //not allow 
        public static string NotAllow = "Bạn không được phép thực hiện hành động này!";

        public static string CheckQuantity = "Số lượng đã đạt tối đa chúng tôi có, đã cập nhật số lượng trong giỏ hàng!";
        public static string TokenOutDate = "Token hết hạn!";
    }
}
