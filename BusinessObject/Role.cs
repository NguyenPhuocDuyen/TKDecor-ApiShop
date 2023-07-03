using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Role
{
    public long RoleId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
