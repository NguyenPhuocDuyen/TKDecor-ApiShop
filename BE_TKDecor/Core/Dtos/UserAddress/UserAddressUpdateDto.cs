using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.UserAddress
{
    public class UserAddressUpdateDto
    {
        public Guid UserAddressId { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        public int CityCode { get; set; }

        [MaxLength(100)]
        public string City { get; set; } = null!;

        public int DistrictCode { get; set; }

        [MaxLength(100)]
        public string District { get; set; } = null!;

        public int WardCode { get; set; }

        [MaxLength(100)]
        public string Ward { get; set; } = null!;

        [MaxLength(100)]
        public string Street { get; set; } = null!;
    }
}
