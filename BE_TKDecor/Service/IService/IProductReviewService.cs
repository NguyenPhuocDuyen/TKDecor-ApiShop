using BE_TKDecor.Core.Dtos.ProductReview;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IProductReviewService
    {
        Task<ApiResponse> ReviewProduct(string? userId, ProductReviewCreateDto dto);
    }
}
