using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IUserService
    {
        Task<ApiResponse> GetById(string? userId);
        Task<ApiResponse> GetAllUser(string? userId);
        Task<ApiResponse> SetRole(Guid userId, UserSetRoleDto dto);
        //Task<ApiResponse> Delete(string userId);
        Task<ApiResponse> UpdateUserInfo(string? userId, UserUpdateDto dto);
        Task<ApiResponse> RequestChangePassword(string? userId);
        Task<ApiResponse> ChangePassword(string? userId, UserChangePasswordDto dto);
    }
}
