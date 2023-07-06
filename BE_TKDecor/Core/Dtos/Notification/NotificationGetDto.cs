namespace BE_TKDecor.Core.Dtos.Notification
{
    public class NotificationGetDto
    {
        public long NotificationId { get; set; }

        public string Message { get; set; } = null!;

        public bool? IsRead { get; set; } = false;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
