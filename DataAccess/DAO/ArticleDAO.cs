using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ArticleDAO
    {
        public static async Task<List<Article>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var list = await context.Articles
                    .Include(x => x.User)
                    .ToListAsync();
                return list;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task<Article?> FindById(int id)
        {
            try
            {
                using var context = new TkdecorContext();
                var article = await context.Articles
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.ArticleId == id);
                return article;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task<Article?> FindByTitle(string title)
        {
            try
            {
                using var context = new TkdecorContext();
                var article = await context.Articles
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Title == title);
                return article;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task<Article?> FindBySlug(string slug)
        {
            try
            {
                using var context = new TkdecorContext();
                var article = await context.Articles
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Slug == slug);
                return article;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task Add(Article article)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(article);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task Update(Article article)
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
