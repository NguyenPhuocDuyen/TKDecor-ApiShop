using BusinessObject;

namespace BE_TKDecor.Core.Dtos.User
{
    public class UserGetDto
    {
        public Guid UserId { get; set; }

        public string Role { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public DateTime BirthDay { get; set; }

        public string Gender { get; set; } = null!;
        
        public string AvatarUrl { get; set; } = null!;

        public bool IsSubscriber { get; set; }

        //public DateTime CreatedAt { get; set; }

        //public DateTime UpdatedAt { get; set; }

        //public bool IsDeleted { get; set; }
    }
}
