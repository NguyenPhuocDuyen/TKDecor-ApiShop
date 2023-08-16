using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Response;
using BusinessObject;

namespace BE_TKDecor.Service.IService
{
    public interface IOrderService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetById(Guid id);
        Task<ApiResponse> GetByIdAndUser(Guid id, Guid userId);
        Task<ApiResponse> UpdateStatusOrder(Guid id, OrderUpdateStatusDto dto);
        Task<ApiResponse> GetAllForUser(Guid userId);
        Task<ApiResponse> MakeOrder(User user, OrderMakeDto dto);
        Task<ApiResponse> UpdateStatusOrderForCus(User user, Guid id, OrderUpdateStatusDto dto);
    }
}
