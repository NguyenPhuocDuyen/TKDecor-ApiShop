using DataAccess.StatusContent;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserSetRoleDto
    {
        public Guid UserId { get; set; }

        [RegularExpression($"^({RoleContent.Seller}|{RoleContent.Customer}|{RoleContent.Admin})$")]
        public string RoleName { get; set; } = null!;
    }
}
