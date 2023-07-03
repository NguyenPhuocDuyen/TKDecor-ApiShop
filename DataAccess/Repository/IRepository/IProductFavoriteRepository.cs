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
        Task<List<ProductFavorite>> FindFavoriteOfUser(long userId);
        Task<ProductFavorite?> FindProductFavorite(long userId, long productId);
        Task Add(ProductFavorite productFavorite);
        Task Update(ProductFavorite productFavorite);
    }
}
