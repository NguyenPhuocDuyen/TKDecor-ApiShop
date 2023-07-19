using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        public async Task Add(Article article) 
            => await ArticleDAO.Add(article);

        public async Task<Article?> FindById(Guid id) 
            => await ArticleDAO.FindById(id);

        public async Task<Article?> FindBySlug(string slug) 
            => await ArticleDAO.FindBySlug(slug);

        public async Task<List<Article>> GetAll()
            => await ArticleDAO.GetAll();

        public async Task Update(Article article)
            => await ArticleDAO.Update(article);
    }
}
