using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    internal class ProductReviewInteractionDAO
    {
        internal static async Task<ProductReviewInteraction?> FindById(long id)
        {
            try
            {
                using var context = new TkdecorContext();
                var productReviewInteration = await context.ProductReviewInteractions
                    .FirstOrDefaultAsync(x => x.ProductReviewInteractionId == id);
                return productReviewInteration;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<ProductReviewInteraction?> FindByUserIdAndProductReviewId(long userId, long productReviewId)
        {
            try
            {
                using var context = new TkdecorContext();
                var productReviewInteration = await context.ProductReviewInteractions
                    .FirstOrDefaultAsync(x => x.UserId == userId
                    && x.ProductReviewId == productReviewId);
                return productReviewInteration;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Add(ProductReviewInteraction productReviewInteraction)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(productReviewInteraction);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(ProductReviewInteraction productReviewInteraction)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(productReviewInteraction);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
