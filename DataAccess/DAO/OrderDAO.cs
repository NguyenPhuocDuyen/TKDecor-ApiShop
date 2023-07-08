using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class OrderDAO
    {
        internal static async Task<List<Order>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var orders = await context.Orders
                    .Include(x => x.OrderStatus)
                    .Include(x => x.User)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                    .ToListAsync();
                return orders;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        } 

        internal static async Task<Order?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                var order = await context.Orders
                    .Include(x => x.OrderStatus)
                    .Include(x => x.User)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                    .FirstOrDefaultAsync(o => o.OrderId == id);
                return order;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<List<Order>> FindByUserId(Guid userId)
        {
            try
            {
                using var context = new TkdecorContext();
                var orders = await context.Orders
                    .Include(x => x.OrderStatus)
                    .Include(x => x.User)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                    .Where(o => o.UserId == userId)
                    .ToListAsync();
                return orders;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Add(Order order)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(order);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(Order order)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(order);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
