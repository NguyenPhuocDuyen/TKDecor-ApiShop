using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; } = false;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
}
