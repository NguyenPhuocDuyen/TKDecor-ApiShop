using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class CartDAO : DAO<Cart>
    {
        internal static async Task<List<Cart>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.Carts
                    .Include(x => x.User)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<List<Cart>> FindCartsByUserId(Guid userId)
        {
            try
            {
                using var context = new TkdecorContext();
                var carts = await context.Carts
                    .Include(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                    .Where(x => x.UserId == userId).ToListAsync();
                return carts;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Cart?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                var cart = await context.Carts
                    .Include(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                    .FirstOrDefaultAsync(x => x.CartId == id);
                return cart;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Cart?> FindByUserIdAndProductId(Guid userId, Guid productId)
        {
            try
            {
                using var context = new TkdecorContext();
                var cart = await context.Carts
                    .Include(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
                return cart;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
