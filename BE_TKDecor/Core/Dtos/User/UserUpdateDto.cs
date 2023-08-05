using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserUpdateDto
    {
        [MaxLength(255)]
        public string FullName { get; set; } = null!;

        [MaxLength(255)]
        public string? AvatarUrl { get; set; }

        public DateTime? BirthDay { get; set; }

        [RegularExpression($"^({SD.GenderMale}|{SD.GenderFemale}|{SD.GenderOther})$")]
        public string? Gender { get; set; }

        public string? Phone { get; set; }
    }
}
