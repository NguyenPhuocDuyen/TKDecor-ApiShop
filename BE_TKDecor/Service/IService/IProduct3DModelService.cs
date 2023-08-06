
using BE_TKDecor.Core.Dtos.Product3DModel;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IProduct3DModelService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetAllByProductId(Guid id);
        Task<ApiResponse> Create(Product3DModelCreateDto dto);
        Task<ApiResponse> Delete(Guid id);
    }
}
