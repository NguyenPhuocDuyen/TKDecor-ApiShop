using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IProductReportService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> Update(Guid id, ProductReportUpdateDto dto);
        Task<ApiResponse> MakeProductReport(Guid userId, ProductReportCreateDto reportDto);
    }
}
