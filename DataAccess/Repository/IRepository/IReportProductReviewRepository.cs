
using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IReportProductReviewRepository
    {
        Task<List<ReportProductReview>> GetAll();
        Task<ReportProductReview?> FindById(int id);
        Task<ReportProductReview?> FindByUserIdAndProductReviewId(int userId, int productReviewId);
        Task Add(ReportProductReview reportProductReview);
        Task Update(ReportProductReview reportProductReview);
    }
}
