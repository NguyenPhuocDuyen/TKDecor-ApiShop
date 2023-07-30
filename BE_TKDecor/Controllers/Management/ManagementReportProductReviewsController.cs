using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.ReportProductReview;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Admin)]
    public class ManagementReportProductReviewsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReportProductReviewRepository _reportProductReview;
        private readonly IProductReviewRepository _productReview;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;

        public ManagementReportProductReviewsController(IMapper mapper,
            IReportProductReviewRepository reportProductReview,
            IProductReviewRepository productReview,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub)
        {
            _mapper = mapper;
            _reportProductReview = reportProductReview;
            _productReview = productReview;
            _notification = notification;
            _hub = hub;
        }

        // GET: api/ManagementReportProductReviews/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _reportProductReview.GetAll();
            reports = reports.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<ReportProductReviewGetDto>>(reports);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/ManagementReportProductReviews/UpdateStatusReport
        [HttpPut("UpdateStatusReport/{id}")]
        public async Task<IActionResult> UpdateStatusReport(Guid id, ReportProductReviewUpdateDto reportDto)
        {
            if (id != reportDto.ReportProductReviewId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var report = await _reportProductReview.FindById(id);
            if (report == null || report.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ReportProductReviewNotFound });

            if (report.ReportStatus != ReportStatus.Pending)
                return BadRequest(new ApiResponse { Message = "Báo cáo đánh giá sản phẩm đã được xử lý!" });

            if (!Enum.TryParse(reportDto.ReportStatus, out ReportStatus status))
                return BadRequest(new ApiResponse { Message = ErrorContent.ReportStatusNotFound });

            report.ReportStatus = status;
            report.ProductReviewReported.IsDelete = true;
            report.UpdatedAt = DateTime.Now;

            var message = "";
            if (status == ReportStatus.Accept)
                message = "được chấp nhận";
            else if (status == ReportStatus.Reject)
                message = "bị từ chối";

            try
            {
                await _reportProductReview.Update(report);

                // update product review to delete
                var productReview = await _productReview.FindById(report.ProductReviewReportedId);
                if (productReview != null)
                {
                    productReview.UpdatedAt = DateTime.Now;
                    productReview.IsDelete = true;
                    await _productReview.Update(productReview);
                }

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = report.UserReportId,
                    Message = $"Báo cáo đánh giá sản phẩm của {report.UserReport.FullName} đã {message}."
                };
                await _notification.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(report.UserReportId.ToString())
                    .SendAsync(Common.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
