using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ArticleDAO : DAO<Article>
    {
        internal static async Task<List<Article>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.Articles
                    .Include(x => x.User)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Article?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.Articles
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.ArticleId == id);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Article?> FindBySlug(string slug)
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.Articles
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Slug == slug);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
