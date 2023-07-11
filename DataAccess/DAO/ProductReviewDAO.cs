using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ProductReviewDAO : DAO<ProductReview>
    {
        internal static async Task<ProductReview?> FindByUserIdAndProductId(Guid userId, Guid productId)
        {
            try
            {
                using var context = new TkdecorContext();
                var productReview = await context.ProductReviews
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
                return productReview;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<ProductReview?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                var productReview = await context.ProductReviews
                    .FirstOrDefaultAsync(x => x.ProductReviewId == id);
                return productReview;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<List<ProductReview>> FindByProductId(Guid productId)
        {
            try
            {
                using var context = new TkdecorContext();
                var productReviews = await context.ProductReviews
                    .Include(x => x.User)
                    .Include(x => x.ProductReviewInteractions)
                    .Where(x => x.ProductId == productId)
                    .ToListAsync();
                return productReviews;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<List<ProductReview>> FindByUserId(Guid userId)
        {
            try
            {
                using var context = new TkdecorContext();
                var productReviews = await context.ProductReviews
                    .Include(x => x.User)
                    .Include(x => x.ProductReviewInteractions)
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
                return productReviews;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
