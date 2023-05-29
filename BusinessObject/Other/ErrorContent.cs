using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Other
{
    public class ErrorContent
    {
        //common
        public static string Error = "Có lỗi xảy ra thử lại sau!";

        public static string Data = "Xung đột dữ liệu thử lại sau!";

        //not found
        public static string UserNotFound = "Không tìm thấy người dùng!";
        public static string ProductNotFound = "Không tìm thấy sản phẩm!";
    }
}
