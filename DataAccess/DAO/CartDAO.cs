using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class CartDAO
    {
        public static async Task<List<Cart>> GetCartsByUserId(int userId)
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

        public static async Task<Cart?> FindById(int id)
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

        public static async Task<Cart?> FindByUserIdAndProductId(int userId, int productId)
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

        public static async Task Add(Cart cart)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(cart);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task Update(Cart cart)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(cart);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task Delete(Cart cart)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Remove(cart);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
