using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IProductService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> Create(ProductCreateDto dto);
        Task<ApiResponse> Update(Guid id, ProductUpdateDto dto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> GetAll(Guid? userId, Guid? categoryId, string search, string sort, int pageIndex, int pageSize);
        Task<ApiResponse> FeaturedProducts(Guid? userId);
        Task<ApiResponse> GetReview(Guid? userId, string slug, string sort, int pageIndex, int pageSize);
        Task<ApiResponse> RelatedProducts(Guid? userId, string slug);
        Task<ApiResponse> GetBySlug(Guid? userId, string slug);
    }
}
