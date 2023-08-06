using BE_TKDecor.Core.Dtos.UserAddress;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IUserAddressService
    {
        Task<ApiResponse> GetUserAddressesForUser(Guid userId);
        Task<ApiResponse> GetUserAddressDefault(Guid userId);
        Task<ApiResponse> SetDefault(Guid userId, UserAddressSetDefaultDto dto);
        Task<ApiResponse> Create(Guid userId, UserAddressCreateDto dto);
        Task<ApiResponse> Delete(Guid userId, Guid id);
        Task<ApiResponse> Update(Guid userId, Guid id, UserAddressUpdateDto dto);
    }
}
