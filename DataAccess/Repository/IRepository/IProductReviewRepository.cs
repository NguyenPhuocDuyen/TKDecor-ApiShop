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
        Task Add(ProductReview productReview);
        Task Update(ProductReview productReview);
        Task<bool> CanReview(int userId, int productId);
        Task<ProductReview?> GetByUserIdAndProductId(int userId, int productId);
        Task<ProductReview?> FindById(int id);
    }
}
