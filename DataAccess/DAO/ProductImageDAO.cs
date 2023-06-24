using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ProductImageDAO
    {
        public static async Task Delete(ProductImage productImage)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Remove(productImage);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
