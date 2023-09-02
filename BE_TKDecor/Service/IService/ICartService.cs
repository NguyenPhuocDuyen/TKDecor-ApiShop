using BE_TKDecor.Core.Dtos.Cart;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface ICartService
    {
        Task<ApiResponse> GetCarts(string? userId);
        Task<ApiResponse> AddProductToCart(string? userId, CartCreateDto dto);
        Task<ApiResponse> UpdateQuantity(string? userId, Guid cartId, CartUpdateDto dto);
        Task<ApiResponse> Delete(string? userId, Guid cartId);
    }
}
