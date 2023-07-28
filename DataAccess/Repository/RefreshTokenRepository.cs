using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> FindByToken(string token)
            => await RefreshTokenDAO.FindByToken(token);

        public async Task Add(RefreshToken refreshToken) 
            => await RefreshTokenDAO.Add(refreshToken);

        public async Task Update(RefreshToken refreshToken)
            => await RefreshTokenDAO.Update(refreshToken);

        public async Task Delete(RefreshToken refreshToken)
            => await RefreshTokenDAO.Delete(refreshToken);

        public async Task<List<RefreshToken>> FindByUserId(Guid userId)
            => await RefreshTokenDAO.FindByUserId(userId);
    }
}
