using BusinessObject;
using DataAccess.StatusContent;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ProductReviewDAO
    {
        internal static async Task<ProductReview?> FindByUserIdAndProductId(long userId, long productId)
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

        internal static async Task<ProductReview?> FindById(long id)
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

        internal static async Task Add(ProductReview productReview)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(productReview);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(ProductReview productReview)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(productReview);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<bool> CanReview(long userId, long productId)
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

        internal static async Task<List<ProductReview>> FindByProductId(long productId)
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

        internal static async Task<List<ProductReview>> FindByUserId(long userId)
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
