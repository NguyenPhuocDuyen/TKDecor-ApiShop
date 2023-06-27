using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class RandomCode
    {
        public static string GenerateRandomCode()
        {
            Random random = new();
            int code = random.Next(1000000, 9999999);
            return code.ToString();
        }
    }
}
