using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IProductReportService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> Update(Guid productReportId, ProductReportUpdateDto dto);
        Task<ApiResponse> MakeProductReport(string? userId, ProductReportCreateDto reportDto);
    }
}
