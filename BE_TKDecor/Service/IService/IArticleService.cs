using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;
using BusinessObject;

namespace BE_TKDecor.Service.IService
{
    public interface IArticleService
    {
        Task<ApiResponse> GetAll(string sort = "default",
            int pageIndex = 1,
            int pageSize = 20);
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetBySlug(string slug);
        Task<ApiResponse> GetById(Guid id);
        Task<ApiResponse> Create(ArticleCreateDto dto, Guid userId);
        Task<ApiResponse> Update(Guid id, ArticleUpdateDto dto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> SetPublish(ArticleSetPublishDto articleDto);
    }
}
