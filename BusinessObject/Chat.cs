namespace BusinessObject;

public partial class Chat : BaseEntity
{
    public Guid ChatId { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
