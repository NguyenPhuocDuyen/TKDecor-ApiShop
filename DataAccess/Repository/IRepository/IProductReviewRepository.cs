using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        Task<List<ProductReview>> FindByUserId(Guid userId);
        Task<List<ProductReview>> FindByProductId(Guid productId);
        Task<ProductReview?> FindByUserIdAndProductId(Guid userId, Guid productId);
    }
}
