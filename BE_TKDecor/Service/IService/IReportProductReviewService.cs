using BE_TKDecor.Core.Dtos.ReportProductReview;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IReportProductReviewService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> Update(Guid reportProductReviewId, ReportProductReviewUpdateDto dto);
        Task<ApiResponse> MakeReportProductReview(string? userId, ReportProductReviewCreateDto dto);
    }
}
