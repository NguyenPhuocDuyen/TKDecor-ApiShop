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
    public class ProductFavoriteRepository : IProductFavoriteRepository
    {
        public async Task<List<ProductFavorite>> GetFavoriteOfUser(int userId)
            => await ProductFavoriteDAO.GetFavoriteOfUser(userId);

        public async Task<ProductFavorite?> FindProductFavorite(int userId, int productId)
            => await ProductFavoriteDAO.FindProductFavorite(userId, productId);

        public async Task Delete(int id)
            => await ProductFavoriteDAO.Delete(id);

        public async Task Add(ProductFavorite productFavorite)
            => await ProductFavoriteDAO.Add(productFavorite);
    }
}
