using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class OrderDAO
    {
        public static async Task Add(Order order)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(order);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
