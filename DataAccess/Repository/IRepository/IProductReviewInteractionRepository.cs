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
        Task<ProductReviewInteraction?> FindById(long id);
        Task<ProductReviewInteraction?> FindByUserIdAndProductReviewId(long userId, long productReviewId);
        Task Add(ProductReviewInteraction productReviewInteraction);
        Task Update(ProductReviewInteraction productReviewInteraction);
    }
}
