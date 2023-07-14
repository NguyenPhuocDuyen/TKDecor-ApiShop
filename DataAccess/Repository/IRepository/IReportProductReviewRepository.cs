
using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IReportProductReviewRepository : IRepository<ReportProductReview>
    {
        Task<ReportProductReview?> FindByUserIdAndProductReviewId(Guid userId, Guid productReviewId);
    }
}
