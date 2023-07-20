using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class MessageChat : BaseEntity
    {
        public Guid MessageChatId { get; set; }
        public Guid RoomChatId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; }

        [ForeignKey(nameof(RoomChatId))]
        public virtual RoomChat RoomChat { get; set; } = null!;

        [ForeignKey(nameof(SenderId))]
        public virtual User Sender { get; set; } = null!;
    }
}
