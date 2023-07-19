using BusinessObject;

namespace BE_TKDecor.Core.Dtos.UserAddress
{
    public class UserAddressGetDto : BaseEntity
    {
        public Guid UserAddressId { get; set; }

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public bool IsDefault { get; set; }
    }
}
