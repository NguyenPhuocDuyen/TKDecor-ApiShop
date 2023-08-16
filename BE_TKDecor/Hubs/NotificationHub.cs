using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BE_TKDecor.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
