using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> FindByToken(string token);
        Task Add(RefreshToken refreshToken);
        Task Update(RefreshToken refreshToken);
    }
}
