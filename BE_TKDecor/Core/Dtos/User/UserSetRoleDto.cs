using DataAccess.StatusContent;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserSetRoleDto
    {
        public int UserId { get; set; }

        [RegularExpression($"^({RoleContent.Seller}|{RoleContent.Customer})$")]
        public string RoleName { get; set; } = null!;
    }
}
