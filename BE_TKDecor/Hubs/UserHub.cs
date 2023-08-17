using Microsoft.AspNetCore.SignalR;

namespace BE_TKDecor.Hubs
{
    //public class UserHub : Hub
    //{
    //    public static int TotalUsers { get; set; } = 0;

    //    public override Task OnConnectedAsync()
    //    {
    //        TotalUsers++;
    //        Clients.All.SendAsync("updateTotalUsers", TotalUsers).GetAwaiter().GetResult();
    //        return base.OnConnectedAsync();
    //    }

    //    public override Task OnDisconnectedAsync(Exception? exception)
    //    {
    //        TotalUsers--;
    //        Clients.All.SendAsync("updateTotalUsers", TotalUsers).GetAwaiter().GetResult();
    //        return base.OnDisconnectedAsync(exception);
    //    }

    //    public int TotalUser()
    //    {
    //        return TotalUsers;
    //    }
    //}
}
