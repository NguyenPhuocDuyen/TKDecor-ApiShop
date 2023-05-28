using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UserRegisterDTO : UserLoginDTO
    {
        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }
    }
}
