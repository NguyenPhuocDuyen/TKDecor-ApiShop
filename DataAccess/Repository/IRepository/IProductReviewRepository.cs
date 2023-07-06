using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductReviewRepository
    {
        Task<List<ProductReview>> FindByUserId(long userId);
        Task<List<ProductReview>> FindByProductId(long productId);
        Task Add(ProductReview productReview);
        Task Update(ProductReview productReview);
        Task<bool> CanReview(long userId, long productId);
        Task<ProductReview?> FindByUserIdAndProductId(long userId, long productId);
        Task<ProductReview?> FindById(long id);
    }
}
