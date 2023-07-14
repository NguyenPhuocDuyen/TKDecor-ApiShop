using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserUpdateDto
    {
        [MaxLength(255)]
        public string FullName { get; set; } = null!;

        [MaxLength(255)]
        public string AvatarUrl { get; set; } = null!;

        public DateTime BirthDay { get; set; }

        [RegularExpression($"^(Male|Female|Other)$")]
        public string Gender { get; set; } = null!;
    }
}
