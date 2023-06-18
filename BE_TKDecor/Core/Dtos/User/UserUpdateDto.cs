namespace BE_TKDecor.Core.Dtos.User
{
    public class UserUpdateDto
    {
        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }
    }
}
