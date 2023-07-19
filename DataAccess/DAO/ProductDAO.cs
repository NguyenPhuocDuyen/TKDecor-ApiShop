using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ProductDAO : DAO<Product>
    {
        internal static async Task<List<Product>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var list = await context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductReviews)
                        //.ThenInclude(x => x.User)
                    .ToListAsync();
                return list;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Product?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                var product = await context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductReviews)
                        //.ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.ProductId == id);
                return product;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Product?> FindBySlug(string slug)
        {
            try
            {
                using var context = new TkdecorContext();
                var product = await context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductReviews)
                        //.ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.Slug == slug);
                return product;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
