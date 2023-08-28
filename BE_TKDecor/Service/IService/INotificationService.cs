using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface INotificationService
    {
        Task<ApiResponse> GetNotificationsForUser(string? userId);
        Task<ApiResponse> ReadAll(string? userId);
    }
}
