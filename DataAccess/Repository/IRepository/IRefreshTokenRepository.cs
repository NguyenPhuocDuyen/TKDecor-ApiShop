using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> FindByToken(string token);
        Task<List<RefreshToken>> FindByUserId(Guid userId);
        Task Add(RefreshToken refreshToken);
        Task Update(RefreshToken refreshToken);
        Task Delete(RefreshToken refreshToken);
    }
}
