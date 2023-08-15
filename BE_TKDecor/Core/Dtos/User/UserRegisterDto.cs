using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserRegisterDto
    {
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [MinLength(7)]
        [MaxLength(20)]
        public string Password { get; set; } = null!;

        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
    }
}
