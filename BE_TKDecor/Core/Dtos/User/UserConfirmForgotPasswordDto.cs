using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserConfirmForgotPasswordDto
    {
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [MaxLength(20)]
        public string Password { get; set; } = null!;

        [MaxLength(20)]
        public string Code { get; set; } = null!;
    }
}
