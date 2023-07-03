using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class ReportProductReviewRepository : IReportProductReviewRepository
    {
        public async Task Add(ReportProductReview reportProductReview)
            => await ReportProductReviewDAO.Add(reportProductReview);

        public async Task<ReportProductReview?> FindByUserIdAndProductReviewId(long userId, long productReviewId)
            => await ReportProductReviewDAO.FindByUserIdAndProductReviewId(userId, productReviewId);

        public async Task<List<ReportProductReview>> GetAll()
            => await ReportProductReviewDAO.GetAll();

        public async Task<ReportProductReview?> FindById(long id)
            => await ReportProductReviewDAO.FindById(id);

        public async Task Update(ReportProductReview reportProductReview)
            => await ReportProductReviewDAO.Update(reportProductReview);
    }
}
