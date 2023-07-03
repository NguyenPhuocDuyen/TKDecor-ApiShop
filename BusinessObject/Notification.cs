using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Notification : BaseEntity
{
    public long NotificationId { get; set; }

    public long UserId { get; set; }

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public virtual User User { get; set; } = null!;
}
