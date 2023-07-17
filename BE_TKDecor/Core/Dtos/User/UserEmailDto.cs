using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserEmailDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
