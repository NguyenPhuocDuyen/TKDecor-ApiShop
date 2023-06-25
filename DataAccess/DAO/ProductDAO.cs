using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ProductDAO
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
                    .ToListAsync();
                return list;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Product?> FindById(int id)
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
                    .FirstOrDefaultAsync(x => x.ProductId == id);
                return product;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Product?> FindByName(string name)
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
                    .FirstOrDefaultAsync(x => x.Name == name);
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
                    .FirstOrDefaultAsync(x => x.Slug == slug);
                return product;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Add(Product product)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(product);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(Product product)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(product);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
