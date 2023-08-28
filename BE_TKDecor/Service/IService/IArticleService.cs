using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IArticleService
    {
        Task<ApiResponse> GetAll(string sort, int pageIndex, int pageSize);
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetBySlug(string slug);
        Task<ApiResponse> GetById(Guid articleId);
        Task<ApiResponse> Create(ArticleCreateDto dto, string? userId);
        Task<ApiResponse> Update(Guid articleId, ArticleUpdateDto dto);
        Task<ApiResponse> Delete(Guid articleId);
        Task<ApiResponse> SetPublish(ArticleSetPublishDto dto);
    }
}
