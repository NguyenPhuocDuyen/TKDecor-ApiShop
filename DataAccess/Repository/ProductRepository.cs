using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        public async Task<List<Product>> GetAll() => await ProductDAO.GetAll();

        public async Task<Product> FindById(int id) => await ProductDAO.FindById(id);

        public async Task<Product> FindByName(string name) => await ProductDAO.FindByName(name);

        public async Task<Product> FindBySlug(string slug) => await ProductDAO.FindBySlug(slug);

        public async Task Add(Product product) => await ProductDAO.Add(product);

        public async Task Update(Product product) => await ProductDAO.Update(product);
    }
}
