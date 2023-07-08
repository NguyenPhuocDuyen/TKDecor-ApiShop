using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ArticleDAO
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

        internal static async Task<Article?> FindByTitle(string title)
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.Articles
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Title == title);
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

        internal static async Task Add(Article article)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(article);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(Article article)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(article);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
