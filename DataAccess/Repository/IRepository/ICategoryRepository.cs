using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAll();
        Task<bool> CheckProductExistsByCateId(Guid categoryId);
        Task<Category?> FindById(Guid categoryId);
        Task<Category?> FindByName(string categoryName);
        Task Add(Category category);
        Task Update(Category category);
    }
}
