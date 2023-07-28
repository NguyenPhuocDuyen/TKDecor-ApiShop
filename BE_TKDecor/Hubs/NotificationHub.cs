using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BE_TKDecor.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        //public async Task JoinGroup(Guid userId)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        //}

        //public async Task LeaveRoom(Guid userId)
        //{
        //    // Rời khỏi group tương ứng với userId
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
        //}
    }
}
