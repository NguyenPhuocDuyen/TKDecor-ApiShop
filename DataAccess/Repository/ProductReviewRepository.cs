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

        public async Task<bool> CanReview(int userId, int productId)
            => await ProductReviewDAO.CanReview(userId, productId);

        public async Task<ProductReview?> FindById(int id)
            => await ProductReviewDAO.FindById(id);

        public async Task<ProductReview?> GetByUserIdAndProductId(int userId, int productId)
            => await ProductReviewDAO.GetByUserIdAndProductId(userId, productId);

        public async Task Update(ProductReview productReview)
            => await ProductReviewDAO.Update(productReview);
    }
}
