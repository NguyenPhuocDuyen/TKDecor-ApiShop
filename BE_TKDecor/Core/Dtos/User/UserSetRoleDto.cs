using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserSetRoleDto
    {
        public Guid UserId { get; set; }

        [RegularExpression($"^({SD.RoleAdmin}|{SD.RoleCustomer})$")]
        public string Role { get; set; } = null!;
    }
}
