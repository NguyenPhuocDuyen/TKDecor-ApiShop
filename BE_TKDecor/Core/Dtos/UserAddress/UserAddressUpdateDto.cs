namespace BE_TKDecor.Core.Dtos.UserAddress
{
    public class UserAddressUpdateDto
    {
        public int UserAddressId { get; set; }

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;
    }
}
