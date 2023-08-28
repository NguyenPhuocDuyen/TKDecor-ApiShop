using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IProductService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> Create(ProductCreateDto dto);
        Task<ApiResponse> Update(Guid productId, ProductUpdateDto dto);
        Task<ApiResponse> Delete(Guid productId);
        Task<ApiResponse> GetAll(string? userId, Guid? categoryId, string search, string sort, int pageIndex, int pageSize);
        Task<ApiResponse> FeaturedProducts(string? userId);
        Task<ApiResponse> GetReview(string? userId, string slug, string sort, int pageIndex, int pageSize);
        Task<ApiResponse> RelatedProducts(string? userId, string slug);
        Task<ApiResponse> GetBySlug(string? userId, string slug);
    }
}
