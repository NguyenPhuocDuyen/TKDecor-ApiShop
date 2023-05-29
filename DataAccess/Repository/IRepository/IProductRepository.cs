using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll();
        Task<Product> FindById(int id);
        Task<Product> FindByName(string Name);
        Task<Product> FindBySlug(string slug);
        Task Add(Product product);
        Task Update(Product product);
    }
}
