using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IArticleRepository
    {
        Task<List<Article>> GetAll();
        Task<Article?> FindById(long id);
        Task<Article?> FindByTitle(string Title);
        Task<Article?> FindBySlug(string slug);
        Task Add(Article article);
        Task Update(Article article);
    }
}
