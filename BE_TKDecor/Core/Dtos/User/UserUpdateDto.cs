using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserUpdateDto
    {
        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public DateTime? BirthDay { get; set; }

        [RegularExpression($"^({SD.GenderMale}|{SD.GenderFemale}|{SD.GenderOther})$")]
        public string? Gender { get; set; }

        [MinLength(10)]
        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}
