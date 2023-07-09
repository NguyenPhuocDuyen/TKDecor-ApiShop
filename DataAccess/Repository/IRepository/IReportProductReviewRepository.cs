
using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IReportProductReviewRepository
    {
        Task<List<ReportProductReview>> GetAll();
        Task<ReportProductReview?> FindById(Guid id);
        Task<ReportProductReview?> FindByUserIdAndProductReviewId(Guid userId, Guid productReviewId);
        Task Add(ReportProductReview reportProductReview);
        Task Update(ReportProductReview reportProductReview);
    }
}
