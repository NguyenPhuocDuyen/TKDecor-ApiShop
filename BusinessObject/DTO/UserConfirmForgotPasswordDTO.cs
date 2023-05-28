using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UserConfirmForgotPasswordDTO : UserLoginDTO
    {
        public string Code { get; set; } = null!;
    }
}
