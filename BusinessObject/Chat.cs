namespace BusinessObject;

public partial class Chat : BaseEntity
{
    public long MessageId { get; set; }

    public long SenderId { get; set; }

    public long ReceiverId { get; set; }

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
