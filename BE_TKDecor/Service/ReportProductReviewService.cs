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
        private ApiResponse _response;

        public ReportProductReviewService(TkdecorContext context,
            IMapper mapper,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _hub = hub;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetAll()
        {
            var reports = await _context.ReportProductReviews
                    .Include(x => x.UserReport)
                    .Include(x => x.ProductReviewReported)
                    .ToListAsync();

            reports = reports.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<ReportProductReviewGetDto>>(reports);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> Update(Guid id, ReportProductReviewUpdateDto dto)
        {
            if (id != dto.ReportProductReviewId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var report = await _context.ReportProductReviews
                    .Include(x => x.UserReport)
                    .Include(x => x.ProductReviewReported)
                    .FirstOrDefaultAsync(x => x.ReportProductReviewId == id);

            if (report == null || report.IsDelete)
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
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }
    }
}
