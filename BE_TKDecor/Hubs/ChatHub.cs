using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

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

        public override async Task<Task> OnConnectedAsync()
        {
            // Lấy thông tin người dùng và các thông tin liên quan khác
            //string userId = Context.UserIdentifier;
            string? userId = Context.User?.FindFirst("UserId")?.Value;

            // Ghi nhận người dùng đã kết nối và thêm vào một nhóm (group) chat tương ứng
            if (userId != null)
            {
                // Kết nối tới nhóm chat tương ứng với userId
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            // Ghi nhận người dùng đã kết nối và thêm vào một nhóm (group) chat tương ứng
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string roomId, string message)
        {
            // Ghi nhận người dùng đã gửi tin nhắn và lấy thông tin người dùng
            string? userId = Context.User?.FindFirst("UserId")?.Value;

            // Lưu tin nhắn vào cơ sở dữ liệu (Nếu cần)
            // ...

            // Gửi tin nhắn cho các thành viên trong nhóm (group) chat tương ứng
            await Clients.Group(roomId).SendAsync("ReceiveMessage", userId, message);
        }
    }
}
