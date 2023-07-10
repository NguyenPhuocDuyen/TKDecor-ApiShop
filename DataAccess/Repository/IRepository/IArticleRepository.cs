using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<Article?> FindByTitle(string Title);
        Task<Article?> FindBySlug(string slug);
    }
}
