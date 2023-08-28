using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Response;
using BusinessObject;

namespace BE_TKDecor.Service.IService
{
    public interface IOrderService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetById(Guid orderId);
        Task<ApiResponse> GetByIdAndUser(Guid orderId, string? userId);
        Task<ApiResponse> UpdateStatusOrder(Guid orderId, OrderUpdateStatusDto dto);
        Task<ApiResponse> GetAllForUser(string? userId);
        Task<ApiResponse> MakeOrder(string? userId, OrderMakeDto dto);
        Task<ApiResponse> UpdateStatusOrderForCus(string? userId, Guid orderId, OrderUpdateStatusDto dto);
    }
}
