using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class Token
    {
        public static string GenerateToken()
        {
            // create code authen email
            byte[] randomBytes = new byte[32];
            using (RNGCryptoServiceProvider rng = new())
            {
                rng.GetBytes(randomBytes);
            }
            string token = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
            return token;
        }

        public static string GenerateRandomCode()
        {
            Random random = new Random();
            int code = random.Next(1000000, 9999999);
            return code.ToString();
        }
    }
}
