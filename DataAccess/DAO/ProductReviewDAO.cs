using BusinessObject;
using DataAccess.StatusContent;
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

        internal static async Task<bool> CanReview(Guid userId, Guid productId)
        {
            try
            {
                using var context = new TkdecorContext();
                var orderDetail = await context.OrderDetails
                    .Include(x => x.Order)
                        .ThenInclude(x => x.OrderStatus)
                    .FirstOrDefaultAsync(x =>
                        x.ProductId == productId
                        && x.Order.UserId == userId
                        && x.Order.OrderStatus.Name == OrderStatusContent.Received);
                if (orderDetail != null) return true;

                return false;
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
                        .ThenInclude(x => x.ProductReviewInteractionStatuses)
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
                        .ThenInclude(x => x.ProductReviewInteractionStatuses)
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
                return productReviews;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
