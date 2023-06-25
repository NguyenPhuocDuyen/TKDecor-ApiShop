using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class RefreshTokenDAO
    {
        public static async Task<RefreshToken?> FindByToken(string token)
        {
            try
            {
                using var context = new TkdecorContext();
                var refreshToken = await context.RefreshTokens.OrderByDescending(x => x.IssuedAt).FirstOrDefaultAsync(x => x.Token == token);
                return refreshToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task Add(RefreshToken refreshToken)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.RefreshTokens.AddAsync(refreshToken);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task Update(RefreshToken refreshToken)
        {
            try
            {
                using var context = new TkdecorContext();
                context.RefreshTokens.Update(refreshToken);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
