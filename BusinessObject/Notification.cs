namespace BusinessObject;

public partial class Notification : BaseEntity
{
    public Guid NotificationId { get; set; }

    public Guid UserId { get; set; }

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public virtual User User { get; set; } = null!;
}
