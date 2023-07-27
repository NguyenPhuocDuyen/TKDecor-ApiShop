using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Notification
{
    public class NotificationGetDto : BaseEntity
    {
        public Guid NotificationId { get; set; }

        public string Message { get; set; } = null!;

        public bool IsRead { get; set; } = false;
    }
}
