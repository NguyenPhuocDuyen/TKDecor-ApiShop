using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.UserAddress
{
    public class UserAddressCreateDto
    {
        [MaxLength(255)]
        public string FullName { get; set; } = null!;

        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;
    }
}
