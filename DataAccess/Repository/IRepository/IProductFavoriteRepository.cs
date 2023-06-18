using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductFavoriteRepository
    {
        Task<List<ProductFavorite>> GetFavoriteOfUser(int userId);
        Task<ProductFavorite?> FindProductFavorite(int userId, int productId);
        Task Add(ProductFavorite productFavorite);
        Task Delete(int id);
    }
}
