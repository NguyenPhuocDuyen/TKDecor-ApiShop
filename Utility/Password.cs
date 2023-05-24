using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Utility
{
    public static class Password
    {
        public static string HashPassword(string password)
        {
            return BCryptNet.HashPassword(password);
        }

        // Hàm này để kiểm tra mật khẩu nhập vào có khớp với mật khẩu đã mã hoá không
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCryptNet.Verify(password, hashedPassword);
        }
    }
}
