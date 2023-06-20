using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task Add(Category category)
            => await CategoryDAO.Add(category);

        public async Task<bool> CheckProductExistsByCateId(int categoryId)
            => await CategoryDAO.CheckProductExistsByCateId(categoryId);

        public async Task<List<Category>> GetAll() => await CategoryDAO.GetAll();

        public async Task<Category?> FindById(int categoryId)
            => await CategoryDAO.FindById(categoryId);

        public async Task<Category?> FindByName(string name)
            => await CategoryDAO.FindByName(name);

        public async Task Update(Category category)
            => await CategoryDAO.Update(category);
    }
}
