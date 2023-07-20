using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly TkdecorContext _context;

        public ChatHub(TkdecorContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        //public async Task SendMessage(Guid chatRoomId, string message)
        //{
        //    var userInfo = new UserInfo(Context.GetHttpContext());
        //    var userId = userInfo.UserId;

        //    if (userId == Guid.Empty)
        //    {
        //        // Xử lý trường hợp xác thực không thành công (không có UserId từ token).
        //        // Ví dụ: quăng một ngoại lệ hoặc gửi thông báo lỗi tới client.
        //        return;
        //    }

        //    // Lưu tin nhắn vào database
        //    var chatMessage = new ChatMessage
        //    {
        //        ChatMessageId = Guid.NewGuid(),
        //        ChatRoomId = chatRoomId,
        //        SenderId = Guid.NewGuid(), // Thay bằng Id của người gửi tin nhắn (Staff hoặc Customer)
        //        Content = message,
        //        IsRead = false
        //    };

        //    _context.ChatMessages.Add(chatMessage);
        //    await _context.SaveChangesAsync();

        //    // Gửi tin nhắn đến tất cả các client trong phòng chat
        //    await Clients.Group(chatRoomId.ToString()).SendAsync("ReceiveMessage", message);
        //}

        //public void JoinChatRoom(Guid chatRoomId)
        //{
        //    Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        //}

        //public void LeaveChatRoom(Guid chatRoomId)
        //{
        //    Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        //}

        //public override Task OnConnectedAsync()
        //{
        //    return base.OnConnectedAsync();
        //}

        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    return base.OnDisconnectedAsync(exception);
        //}
    }
}
