using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    internal class ProductReviewInteractionDAO : DAO<ProductReviewInteraction>
    {
        internal static async Task<ProductReviewInteraction?> FindById(Guid id)
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

        internal static async Task<ProductReviewInteraction?> FindByUserIdAndProductReviewId(Guid userId, Guid productReviewId)
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

        internal static async Task<List<ProductReviewInteraction>> FindByUserId(Guid userId)
        {
            try
            {
                using var context = new TkdecorContext();
                var productReviewInterations = await context.ProductReviewInteractions
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
                return productReviewInterations;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
