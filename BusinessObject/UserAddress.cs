using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class UserAddress : BaseEntity
{
    public long UserAddressId { get; set; }

    public long UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool IsDefault { get; set; } = false;

    public virtual User User { get; set; } = null!;
}
