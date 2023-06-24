using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserUpdateDto
    {
        [MaxLength(255)]
        public string FullName { get; set; } = null!;

        [MaxLength(255)]
        public string? AvatarUrl { get; set; }
    }
}
