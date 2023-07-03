using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class CartDAO
    {
        internal static async Task<List<Cart>> FindCartsByUserId(long userId)
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

        internal static async Task<Cart?> FindById(long id)
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

        internal static async Task<Cart?> FindByUserIdAndProductId(long userId, long productId)
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

        internal static async Task Add(Cart cart)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(cart);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(Cart cart)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(cart);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
