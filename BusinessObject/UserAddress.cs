using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class UserAddress
{
    public int UserAddressId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool? IsDefault { get; set; } = false;

    public virtual User User { get; set; } = null!;
}
