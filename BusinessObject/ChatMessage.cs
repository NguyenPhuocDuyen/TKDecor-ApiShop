using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject
{
    public class ChatMessage : BaseEntity
    {
        public Guid ChatMessageId { get; set; }
        public Guid ChatRoomId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; } = false;

        [ForeignKey(nameof(ChatRoomId))]
        public virtual ChatRoom ChatRoom { get; set; } = null!;

        [ForeignKey(nameof(SenderId))]
        public virtual User Sender { get; set; } = null!;
    }
}
