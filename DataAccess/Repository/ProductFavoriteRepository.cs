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
        public async Task<List<ProductFavorite>> FindFavoriteOfUser(long userId)
            => await ProductFavoriteDAO.FindFavoriteOfUser(userId);

        public async Task<ProductFavorite?> FindProductFavorite(long userId, long productId)
            => await ProductFavoriteDAO.FindProductFavorite(userId, productId);

        public async Task Update(ProductFavorite productFavorite)
            => await ProductFavoriteDAO.Update(productFavorite);

        public async Task Add(ProductFavorite productFavorite)
            => await ProductFavoriteDAO.Add(productFavorite);
    }
}
