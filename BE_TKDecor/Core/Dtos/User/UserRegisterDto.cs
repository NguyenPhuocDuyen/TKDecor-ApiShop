using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserRegisterDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;

        [MaxLength(100)]
        public string Password { get; set; } = null!;

        [MaxLength(255)]
        public string FullName { get; set; } = null!;

        //public DateTime BirthDay { get; set; }

        //public string Phone { get; set; } = null!;

        //[RegularExpression($"^({SD.GenderMale}|{SD.GenderFemale}|{SD.GenderOther})$")]
        //public string Gender { get; set; } = null!;
    }
}
