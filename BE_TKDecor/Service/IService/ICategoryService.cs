using BE_TKDecor.Core.Dtos.Category;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface ICategoryService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> Create(CategoryCreateDto dto);
        Task<ApiResponse> Update(Guid id, CategoryUpdateDto categoryDto);
        Task<ApiResponse> Delete(Guid id);
    }
}
