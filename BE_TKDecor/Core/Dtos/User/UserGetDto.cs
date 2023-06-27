using BusinessObject;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserGetDto
    {
        public int UserId { get; set; }

        //public int RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public bool? IsSubscriber { get; set; } = false;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDelete { get; set; } = false;
    }
}
