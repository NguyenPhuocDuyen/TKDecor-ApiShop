using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductReviewInteractionRepository : IRepository<ProductReviewInteraction>
    {
        Task<List<ProductReviewInteraction>> FindByUserId(Guid userId);
        Task<ProductReviewInteraction?> FindByUserIdAndProductReviewId(Guid userId, Guid productReviewId);
    }
}
