using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserChangePasswordDto
    {
        [MaxLength(255)]
        public string Password { get; set; } = null!;
        [MaxLength(255)]
        public string NewPassword { get; set; } = null!;
    }
}
