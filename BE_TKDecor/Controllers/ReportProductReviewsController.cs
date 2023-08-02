using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.ReportProductReview;
using Utility;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Dtos.Notification;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using BE_TKDecor.Hubs;

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
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMapper _mapper;

        public ReportProductReviewsController(IUserRepository user,
            IProductReviewRepository productReview,
            IReportProductReviewRepository reportProductReview,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub,
            IMapper mapper
            )
        {
            _user = user;
            _productReview = productReview;
            _reportProductReview = reportProductReview;
            _notification = notification;
            _hub = hub;
            _mapper = mapper;
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

            if (report != null && report.ReportStatus == SD.ReportPending)
                isAdd = false;

            try
            {
                if (isAdd)
                {
                    ReportProductReview newReport = new()
                    {
                        UserReportId = user.UserId,
                        ProductReviewReportedId = productReview.ProductReviewId,
                        ReportStatus = SD.ReportPending,
                        Reason = reportDto.Reason
                    };
                    await _reportProductReview.Add(newReport);
                }
                else
                {
                    report.IsDelete = false;
                    report.Reason = reportDto.Reason;
                    report.ReportStatus = SD.ReportPending;
                    report.UpdatedAt = DateTime.Now;
                    await _reportProductReview.Update(report);
                }

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Đã báo cáo đánh giá của {productReview.User.FullName} thành công"
                };
                await _notification.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                // add notification for staff and admin
                var listStaffOrAdmin = (await _user.GetAll()).Where(x => x.Role != SD.RoleCustomer);
                foreach (var staff in listStaffOrAdmin)
                {
                    // add notification for user
                    Notification notiForStaffOrAdmin = new()
                    {
                        UserId = staff.UserId,
                        Message = $"{user.Email} đã báo cáo đánh giá của {productReview.User.FullName}"
                    };
                    await _notification.Add(notiForStaffOrAdmin);
                    // notification signalR
                    await _hub.Clients.User(staff.UserId.ToString())
                        .SendAsync(SD.NewNotification,
                        _mapper.Map<NotificationGetDto>(notiForStaffOrAdmin));
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
