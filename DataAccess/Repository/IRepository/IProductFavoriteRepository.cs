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
        Task<List<ProductFavorite>> FindFavoriteOfUser(Guid userId);
        Task<ProductFavorite?> FindProductFavorite(Guid userId, Guid productId);
        Task Add(ProductFavorite productFavorite);
        Task Update(ProductFavorite productFavorite);
    }
}
