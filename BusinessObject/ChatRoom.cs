using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class ChatRoom : BaseEntity
    {
        public Guid ChatRoomId { get; set; }
        public Guid StaffId { get; set; }
        public Guid CustomerId { get; set; }
        public bool IsClose { get; set; }

        [ForeignKey(nameof(StaffId))]
        public virtual User Staff { get; set; } = null!;

        [ForeignKey(nameof(CustomerId))]
        public virtual User Customer { get; set; } = null!;

        public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}
