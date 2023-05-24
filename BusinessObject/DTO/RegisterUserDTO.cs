using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class RegisterUserDTO : LoginUserDTO
    {
        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }
    }
}
