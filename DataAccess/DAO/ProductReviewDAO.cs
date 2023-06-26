using BusinessObject;
using DataAccess.StatusContent;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ProductReviewDAO
    {
        internal static async Task<ProductReview?> GetByUserIdAndProductId(int userId, int productId)
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

        internal static async Task<ProductReview?> FindById(int id)
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

        internal static async Task<bool> CanReview(int userId, int productId)
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
    }
}
