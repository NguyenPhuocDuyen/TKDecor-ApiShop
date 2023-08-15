using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserChangePasswordDto
    {
        [MaxLength(20)]
        public string Password { get; set; } = null!;
        [MaxLength(20)]
        public string NewPassword { get; set; } = null!;
        [MaxLength(20)]
        public string Code { get; set; } = null!;
    }
}
