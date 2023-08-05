using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> FindByToken(string token);
        Task<List<RefreshToken>> FindByUserId(Guid userId);
        Task Delete(RefreshToken refreshToken);
        Task Add(RefreshToken refreshToken);
        Task Update(RefreshToken refreshToken);
    }
}
