using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class CategoryDAO : DAO<Category>
    {
        internal static async Task<List<Category>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var listCategories = await context.Categories
                    .Include(x => x.Products)
                    .ToListAsync();
                return listCategories;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Category?> FindById(Guid categoryId)
        {
            try
            {
                using var context = new TkdecorContext();
                var category = await context.Categories
                    .Include(x => x.Products)
                    .FirstOrDefaultAsync(x => x.CategoryId == categoryId);
                return category;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Category?> FindByName(string name)
        {
            try
            {
                using var context = new TkdecorContext();
                var category = await context.Categories
                    .Include(x => x.Products)
                    .FirstOrDefaultAsync(x => x.Name.Equals(name));
                return category;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
