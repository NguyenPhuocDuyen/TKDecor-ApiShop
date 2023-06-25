using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ProductFavoriteDAO
    {
        internal static async Task<List<ProductFavorite>> GetFavoriteOfUser(int userId)
        {
            try
            {
                using var _context = new TkdecorContext();
                var productFavorite = await _context.ProductFavorites
                    .Where(x => x.UserId == userId).ToListAsync();
                return productFavorite;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<ProductFavorite?> FindProductFavorite(int userId, int productId)
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

        internal static async Task Delete(ProductFavorite productFavorite)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Remove(productFavorite);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Add(ProductFavorite productFavorite)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(productFavorite);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
