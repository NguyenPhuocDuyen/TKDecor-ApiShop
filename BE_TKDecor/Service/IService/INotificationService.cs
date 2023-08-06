using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface INotificationService
    {
        Task<ApiResponse> GetNotificationsForUser(Guid userId);
        Task<ApiResponse> ReadAll(Guid userId);
    }
}
