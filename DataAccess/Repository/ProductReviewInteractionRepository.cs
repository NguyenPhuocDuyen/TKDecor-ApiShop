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
    public class ProductReviewInteractionRepository : IProductReviewInteractionRepository
    {
        public async Task Add(ProductReviewInteraction productReviewInteraction)
            => await ProductReviewInteractionDAO.Add(productReviewInteraction);

        public async Task<ProductReviewInteraction?> FindById(Guid id)
            => await ProductReviewInteractionDAO.FindById(id);

        public async Task<List<ProductReviewInteraction>> FindByUserId(Guid userId)
            => await ProductReviewInteractionDAO.FindByUserId(userId);

        public async Task<ProductReviewInteraction?> FindByUserIdAndProductReviewId(Guid userId, Guid productReviewId)
            => await ProductReviewInteractionDAO.FindByUserIdAndProductReviewId(userId, productReviewId);

        public Task<List<ProductReviewInteraction>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task Update(ProductReviewInteraction productReviewInteraction)
            => await ProductReviewInteractionDAO.Update(productReviewInteraction);
    }
}
