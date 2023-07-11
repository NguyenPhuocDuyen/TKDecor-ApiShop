using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        public async Task Add(ProductReview productReview)
            => await ProductReviewDAO.Add(productReview);

        public async Task<ProductReview?> FindById(Guid id)
            => await ProductReviewDAO.FindById(id);

        public async Task<ProductReview?> FindByUserIdAndProductId(Guid userId, Guid productId)
            => await ProductReviewDAO.FindByUserIdAndProductId(userId, productId);

        public async Task<List<ProductReview>> FindByProductId(Guid productId)
            => await ProductReviewDAO.FindByProductId(productId);

        public async Task Update(ProductReview productReview)
            => await ProductReviewDAO.Update(productReview);

        public async Task<List<ProductReview>> FindByUserId(Guid userId)
            => await ProductReviewDAO.FindByUserId(userId);

        public Task<List<ProductReview>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
