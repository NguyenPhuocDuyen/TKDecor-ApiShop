using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    internal class ProductReviewInteractionStatusDAO
    {
        internal static async Task<List<ProductReviewInteractionStatus>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var productReviewInteractionStatuses = await context.ProductReviewInteractionStatuses
                    .ToListAsync();
                return productReviewInteractionStatuses;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
