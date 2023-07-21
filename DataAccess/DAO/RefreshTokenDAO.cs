using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class RefreshTokenDAO : DAO<RefreshToken>
    {
        internal static async Task<RefreshToken?> FindByToken(string token)
        {
            try
            {
                using var context = new TkdecorContext();
                var refreshToken = await context.RefreshTokens.OrderByDescending(x => x.IssuedAt).FirstOrDefaultAsync(x => x.Token == token);
                return refreshToken;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Delete(RefreshToken refreshToken)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Remove(refreshToken);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<List<RefreshToken>> FindByUserId(Guid userId)
        {
            try
            {
                using var context = new TkdecorContext();
                var refreshTokens = await context.RefreshTokens
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
                return refreshTokens;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
