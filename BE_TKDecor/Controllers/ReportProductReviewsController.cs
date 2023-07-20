using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.ReportProductReview;
using Utility.SD;
using Microsoft.AspNetCore.Authorization;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportProductReviewsController : ControllerBase
    {
        private readonly IUserRepository _user;
        private readonly IProductReviewRepository _productReview;
        private readonly IReportProductReviewRepository _reportProductReview;

        public ReportProductReviewsController(IUserRepository user,
            IProductReviewRepository productReview,
            IReportProductReviewRepository reportProductReview)
        {
            _user = user;
            _productReview = productReview;
            _reportProductReview = reportProductReview;
        }

        // POST: api/ReportProductReviews/MakeReport
        [HttpPost("MakeReport")]
        public async Task<ActionResult<ReportProductReview>> MakeReportProductReview(ReportProductReviewCreateDto reportDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var productReview = await _productReview.FindById(reportDto.ProductReviewReportedId);
            if (productReview == null || productReview.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReviewNotFound });

            var report = await _reportProductReview
                .FindByUserIdAndProductReviewId(user.UserId, reportDto.ProductReviewReportedId);

            bool isAdd = true;

            if (report != null && report.ReportStatus == ReportStatus.Pending)
                isAdd = false;

            try
            {
                if (isAdd)
                {
                    ReportProductReview newReport = new()
                    {
                        UserReportId = user.UserId,
                        ProductReviewReportedId = productReview.ProductReviewId,
                        ReportStatus = ReportStatus.Pending,
                        Reason = reportDto.Reason
                    };
                    await _reportProductReview.Add(newReport);
                }
                else
                {
                    report.IsDelete = false;
                    report.Reason = reportDto.Reason;
                    report.UpdatedAt = DateTime.Now;
                    await _reportProductReview.Update(report);
                }

                return Ok(new ApiResponse { Success = true });
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
                    return await _user.FindById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
