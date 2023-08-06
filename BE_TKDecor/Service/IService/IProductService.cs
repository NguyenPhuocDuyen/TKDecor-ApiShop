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
    }
}
