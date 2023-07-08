using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductReviewInteractionRepository
    {
        Task<List<ProductReviewInteraction>> FindByUserId(Guid userId);
        Task<ProductReviewInteraction?> FindById(Guid id);
        Task<ProductReviewInteraction?> FindByUserIdAndProductReviewId(Guid userId, Guid productReviewId);
        Task Add(ProductReviewInteraction productReviewInteraction);
        Task Update(ProductReviewInteraction productReviewInteraction);
    }
}
