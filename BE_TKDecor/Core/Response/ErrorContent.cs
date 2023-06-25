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
        public static string Error = "An error occurred, try again later!";

        public static string Data = "Data conflict try again later!";
        public static string NotMatchId = "Not match id!";

        //not found
        public static string UserNotFound = "User not found!";
        public static string ProductNotFound = "Products not found!";
        public static string ArticleNotFound = "Article not found!";
        public static string AddressNotFound = "Address not found!";
        public static string CategoryNotFound = "Category not found!";
        public static string CouponNotFound = "Coupon not found!";
        public static string CartNotFound = "Cart not found!";

        public static string AccountIncorrect = "Email or password is incorrect!";
    }
}
