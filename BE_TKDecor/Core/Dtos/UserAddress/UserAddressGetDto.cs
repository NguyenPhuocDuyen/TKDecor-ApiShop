namespace BE_TKDecor.Core.Dtos.UserAddress
{
    public class UserAddressGetDto
    {
        public long UserAddressId { get; set; }

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDefault { get; set; } = false;
    }
}
