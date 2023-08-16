using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using BusinessObject;

namespace BE_TKDecor.Service.IService
{
    public interface IUserService
    {
        Task<User?> GetById(Guid id);
        Task<ApiResponse> GetAllUser(Guid userId);
        Task<ApiResponse> SetRole(Guid userId, UserSetRoleDto dto);
        Task<ApiResponse> Delete(Guid userId);
        Task<ApiResponse> UpdateUserInfo(User user);
        Task<ApiResponse> RequestChangePassword(User user);
        Task<ApiResponse> ChangePassword(User user, UserChangePasswordDto dto);
    }
}
