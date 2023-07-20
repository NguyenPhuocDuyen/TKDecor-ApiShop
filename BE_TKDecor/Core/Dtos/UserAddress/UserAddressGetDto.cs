using BusinessObject;

namespace BE_TKDecor.Core.Dtos.UserAddress
{
    public class UserAddressGetDto : BaseEntity
    {
        public Guid UserAddressId { get; set; }

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public int CityCode { get; set; }

        public string City { get; set; } = null!;

        public int DistrictCode { get; set; }

        public string District { get; set; } = null!;

        public int WardCode { get; set; }

        public string Ward { get; set; } = null!;

        public string Street { get; set; } = null!;

        public bool IsDefault { get; set; }
    }
}
