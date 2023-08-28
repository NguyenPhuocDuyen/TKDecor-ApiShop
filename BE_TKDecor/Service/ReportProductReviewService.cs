using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.ReportProductReview;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class ReportProductReviewService : IReportProductReviewService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly ApiResponse _response;

        public ReportProductReviewService(TkdecorContext context, IMapper mapper, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _hub = hub;
            _response = new ApiResponse();
        }

        // get all report
        public async Task<ApiResponse> GetAll()
        {
            var reports = await _context.ReportProductReviews
                .Include(x => x.UserReport)
                .Include(x => x.ProductReviewReported)
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            try
            {
                var result = _mapper.Map<List<ReportProductReviewGetDto>>(reports);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // make report product review
        public async Task<ApiResponse> MakeReportProductReview(string? userId, ReportProductReviewCreateDto reportDto)
        {
            if (userId is null)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            var productReview = await _context.ProductReviews
                    .Include(x => x.OrderDetail)
                        .ThenInclude(x => x.Order)
                            .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.ProductReviewId == reportDto.ProductReviewReportedId
                                            && !x.IsDelete);

            if (productReview is null)
            {
                _response.Message = ErrorContent.ProductReviewNotFound;
                return _response;
            }

            var report = await _context.ReportProductReviews
                .FirstOrDefaultAsync(x => x.UserReportId.ToString() == userId
                && x.ProductReviewReportedId == productReview.ProductReviewId);

            try
            {
                if (report is null || report.ReportStatus != SD.ReportPending)
                {
                    ReportProductReview newReport = new()
                    {
                        UserReportId = Guid.Parse(userId),
                        ProductReviewReportedId = productReview.ProductReviewId,
                        ReportStatus = SD.ReportPending,
                        Reason = reportDto.Reason
                    };
                    _context.ReportProductReviews.Add(newReport);
                }
                else
                {
                    report.IsDelete = false;
                    report.Reason = reportDto.Reason;
                    report.ReportStatus = SD.ReportPending;
                    report.UpdatedAt = DateTime.Now;
                    _context.ReportProductReviews.Update(report);
                }

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = Guid.Parse(userId),
                    Message = $"Đã báo cáo đánh giá của {productReview.OrderDetail.Order.User.FullName} thành công"
                };
                _context.Notifications.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(userId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                // add notification for staff and admin
                var admins = await _context.Users
                    .Where(x => x.Role == SD.RoleAdmin && !x.IsDelete)
                    .ToListAsync();
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId.ToString() == userId);
                foreach (var admin in admins)
                {
                    // add notification for user
                    Notification notiForAdmin = new()
                    {
                        UserId = admin.UserId,
                        Message = $"{user?.Email} đã báo cáo đánh giá của {productReview.OrderDetail.Order.User.FullName}"
                    };
                    _context.Notifications.Add(notiForAdmin);
                    // notification signalR
                    await _hub.Clients.User(admin.UserId.ToString())
                        .SendAsync(SD.NewNotification,
                        _mapper.Map<NotificationGetDto>(notiForAdmin));
                }
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // update status of report product review
        public async Task<ApiResponse> Update(Guid reportProductReviewId, ReportProductReviewUpdateDto dto)
        {
            if (reportProductReviewId != dto.ReportProductReviewId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var report = await _context.ReportProductReviews
                    .Include(x => x.UserReport)
                    .Include(x => x.ProductReviewReported)
                    .FirstOrDefaultAsync(x => x.ReportProductReviewId == reportProductReviewId
                                        && !x.IsDelete);

            if (report is null)
            {
                _response.Message = ErrorContent.ReportProductReviewNotFound;
                return _response;
            }

            if (report.ReportStatus != SD.ReportPending)
            {
                _response.Message = "Báo cáo đánh giá sản phẩm đã được xử lý!";
                return _response;
            }

            report.ReportStatus = dto.ReportStatus;
            report.ProductReviewReported.IsDelete = true;
            report.UpdatedAt = DateTime.Now;

            var message = "";
            if (dto.ReportStatus == SD.ReportAccept)
                message = "được chấp nhận";
            else if (dto.ReportStatus == SD.ReportReject)
                message = "bị từ chối";

            try
            {
                if (dto.ReportStatus == SD.ReportAccept)
                {
                    report.ProductReviewReported.UpdatedAt = DateTime.Now;
                    report.ProductReviewReported.IsDelete = true;
                }
                _context.ReportProductReviews.Update(report);

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = report.UserReportId,
                    Message = $"Báo cáo đánh giá sản phẩm của {report.UserReport.FullName} đã {message}."
                };
                _context.Notifications.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(report.UserReportId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
