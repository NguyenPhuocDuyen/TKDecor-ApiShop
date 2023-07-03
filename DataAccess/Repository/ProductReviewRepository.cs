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
    public class ProductReviewRepository : IProductReviewRepository
    {
        public async Task Add(ProductReview productReview)
            => await ProductReviewDAO.Add(productReview);

        public async Task<bool> CanReview(long userId, long productId)
            => await ProductReviewDAO.CanReview(userId, productId);

        public async Task<ProductReview?> FindById(long id)
            => await ProductReviewDAO.FindById(id);

        public async Task<ProductReview?> FindByUserIdAndProductId(long userId, long productId)
            => await ProductReviewDAO.FindByUserIdAndProductId(userId, productId);

        public async Task Update(ProductReview productReview)
            => await ProductReviewDAO.Update(productReview);
    }
}
