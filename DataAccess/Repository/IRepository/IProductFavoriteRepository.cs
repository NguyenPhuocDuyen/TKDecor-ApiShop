using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductFavoriteRepository : IRepository<ProductFavorite>
    {
        Task<List<ProductFavorite>> FindByUserId(Guid userId);
        Task<ProductFavorite?> FindByUserIdAndProductId(Guid userId, Guid productId);
    }
}
