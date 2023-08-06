using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IOrderService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetById(Guid id);
        Task<ApiResponse> UpdateStatusOrder(Guid id, OrderUpdateStatusDto dto);
    }
}
