﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserConfirmForgotPasswordDto
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Code { get; set; } = null!;
    }
}
