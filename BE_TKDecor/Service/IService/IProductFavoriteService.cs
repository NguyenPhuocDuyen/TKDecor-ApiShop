using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IProductFavoriteService
    {
        Task<ApiResponse> GetFavoriteOfUser(Guid userId, int pageIndex, int pageSize);
        Task<ApiResponse> SetFavorite(Guid userId, FavoriteSetDto dto);
    }
}
