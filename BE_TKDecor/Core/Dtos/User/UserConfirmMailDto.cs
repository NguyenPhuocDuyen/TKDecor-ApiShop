using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserConfirmMailDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        [MaxLength(100)]
        public string Code { get; set; } = null!;
    }
}
