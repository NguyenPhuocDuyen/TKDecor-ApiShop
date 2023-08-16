using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserChangePasswordDto
    {
        [MinLength(7)]
        [MaxLength(20)]
        public string Password { get; set; } = null!;
        [MinLength(7)]
        [MaxLength(20)]
        public string NewPassword { get; set; } = null!;
        [MinLength(7)]
        [MaxLength(20)]
        public string Code { get; set; } = null!;
    }
}
