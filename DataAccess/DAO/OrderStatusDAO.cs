using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class OrderStatusDAO
    {
        public static async Task<OrderStatus?> FindByName(string name)
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.OrderStatuses
                    .FirstOrDefaultAsync(x => x.Name == name);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
