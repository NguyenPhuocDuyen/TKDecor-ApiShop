using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.ReportProductReview;
using DataAccess.Repository;
using DataAccess.StatusContent;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportProductReviewsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IReportProductReviewRepository _reportProductReviewRepository;
        private readonly IReportStatusRepository _reportStatusRepository;

        public ReportProductReviewsController(IUserRepository userRepository,
            IProductReviewRepository productReviewRepository,
            IReportProductReviewRepository reportProductReviewRepository,
            IReportStatusRepository reportStatusRepository)
        {
            _userRepository = userRepository;
            _productReviewRepository = productReviewRepository;
            _reportProductReviewRepository = reportProductReviewRepository;
            _reportStatusRepository = reportStatusRepository;
        }

        // POST: api/ReportProductReviews/MakeReport
        [HttpPost("MakeReport")]
        public async Task<ActionResult<ReportProductReview>> MakeReportProductReview(ReportProductReviewCreateDto reportDto)
        {
            var user = await GetUser();
            if (user == null)
                return BadRequest(new ApiResponse { Message = ErrorContent.UserNotFound });

            var productReview = await _productReviewRepository.FindById(reportDto.ProductReviewReportedId);
            if (productReview == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReviewNotFound });

            var report = await _reportProductReviewRepository
                .FindByUserIdAndProductReviewId(user.UserId, reportDto.ProductReviewReportedId);

            var reportStatus = await _reportStatusRepository.GetAll();
            var statusPeding = reportStatus.FirstOrDefault(x => x.Name == ReportStatusContent.Pending);
            if (statusPeding == null)
                return BadRequest(new ApiResponse { Message = ErrorContent.Error });

            bool isAdd = true;
            // create a new report of that user for that product review
            // if there is no report that product review is in the peding state
            if (report == null || (report.ReportStatus.Name != ReportStatusContent.Pending))
            {
                report = new ReportProductReview()
                {
                    UserReportId = user.UserId,
                    UserReport = user,
                    ProductReviewReportedId = productReview.ProductReviewId,
                    ProductReviewReported = productReview,
                    ReportStatusId = statusPeding.ReportStatusId,
                    ReportStatus = statusPeding,
                    Reason = reportDto.Reason
                };
            }
            else
            {
                isAdd = false;
                report.Reason = reportDto.Reason;
                report.UpdatedAt = DateTime.UtcNow;
            }

            try
            {
                if (isAdd)
                {
                    await _reportProductReviewRepository.Add(report);
                }
                else
                {
                    await _reportProductReviewRepository.Update(report);
                }

                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _userRepository.FindById(int.Parse(userId));
            }
            return null;
        }
    }
}
