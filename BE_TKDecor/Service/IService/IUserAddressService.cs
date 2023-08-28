using BE_TKDecor.Core.Dtos.UserAddress;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IUserAddressService
    {
        Task<ApiResponse> GetUserAddressesForUser(string? userId);
        Task<ApiResponse> GetUserAddressDefault(string? userId);
        Task<ApiResponse> SetDefault(string? userId, UserAddressSetDefaultDto dto);
        Task<ApiResponse> Create(string? userId, UserAddressCreateDto dto);
        Task<ApiResponse> Delete(string? userId, Guid userAddressId);
        Task<ApiResponse> Update(string? userId, Guid userAddressId, UserAddressUpdateDto dto);
    }
}
