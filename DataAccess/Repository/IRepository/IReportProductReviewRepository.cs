
using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IReportProductReviewRepository
    {
        Task<List<ReportProductReview>> GetAll();
        Task<ReportProductReview?> FindById(long id);
        Task<ReportProductReview?> FindByUserIdAndProductReviewId(long userId, long productReviewId);
        Task Add(ReportProductReview reportProductReview);
        Task Update(ReportProductReview reportProductReview);
    }
}
