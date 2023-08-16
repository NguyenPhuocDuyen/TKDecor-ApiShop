using BE_TKDecor.Core.Dtos.Cart;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface ICartService
    {
        Task<ApiResponse> GetCartsForUser(Guid userId);
        Task<ApiResponse> AddProductToCart(Guid userId, CartCreateDto dto);
        Task<ApiResponse> UpdateQuantity(Guid userId, Guid id, CartUpdateDto cartDto);
        Task<ApiResponse> Delete(Guid userId, Guid id);
    }
}
