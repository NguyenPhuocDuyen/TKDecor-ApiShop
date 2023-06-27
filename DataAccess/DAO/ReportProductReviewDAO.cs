using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class ReportProductReviewDAO
    {
        internal static async Task<List<ReportProductReview>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var reports = await context.ReportProductReviews
                    .Include(x => x.ReportStatus)
                    .Include(x => x.UserReport)
                    .Include(x => x.ProductReviewReported)
                    .ToListAsync();
                return reports;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<ReportProductReview?> FindById(int id)
        {
            try
            {
                using var context = new TkdecorContext();
                var report = await context.ReportProductReviews
                    .Include(x => x.ReportStatus)
                    .Include(x => x.UserReport)
                    .Include(x => x.ProductReviewReported)
                    .FirstOrDefaultAsync(x => x.ReportProductReviewId == id);
                return report;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<ReportProductReview?> FindByUserIdAndProductReviewId(int userId, int productReviewId)
        {
            try
            {
                using var context = new TkdecorContext();
                var report = await context.ReportProductReviews
                    .Include(x => x.ReportStatus)
                    .Include(x => x.UserReport)
                    .Include(x => x.ProductReviewReported)
                    .FirstOrDefaultAsync(x => x.UserReportId == userId
                    && x.ProductReviewReportedId == productReviewId);
                return report;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Add(ReportProductReview reportProductReview)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(reportProductReview);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(ReportProductReview reportProductReview)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(reportProductReview);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
