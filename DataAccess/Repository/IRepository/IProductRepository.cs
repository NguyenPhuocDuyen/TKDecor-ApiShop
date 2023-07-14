using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> FindByName(string Name);
        Task<Product?> FindBySlug(string slug);
    }
}
