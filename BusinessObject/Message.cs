using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Message
{
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string Content { get; set; } = null!;

    public bool? IsRead { get; set; } = false;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
