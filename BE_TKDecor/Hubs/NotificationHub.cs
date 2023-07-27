using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BE_TKDecor.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        //// Phương thức này được gọi khi có thông báo mới
        //public async Task SendNotificationToUser(string userId, string notificationMessage)
        //{
        //    await Clients.User(userId).SendAsync("ReceiveNotification", notificationMessage);
        //}
    }
}
