using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ProductFavoriteDAO : DAO<ProductFavorite>
    {
        internal static async Task<List<ProductFavorite>> GetAll()
        {
            try
            {
                using var _context = new TkdecorContext();
                var productFavorite = await _context.ProductFavorites
                    .ToListAsync();
                return productFavorite;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<List<ProductFavorite>> FindByUserId(Guid userId)
        {
            try
            {
                using var _context = new TkdecorContext();
                var productFavorite = await _context.ProductFavorites
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
                return productFavorite;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<ProductFavorite?> FindByUserIdAndProductId(Guid userId, Guid productId)
        {
            try
            {
                using var _context = new TkdecorContext();
                var productFavorite = await _context.ProductFavorites
                    .FirstOrDefaultAsync(x => x.UserId == userId
                        && x.ProductId == productId);
                return productFavorite;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
