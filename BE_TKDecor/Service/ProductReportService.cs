using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class ProductReportService : IProductReportService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hub;
        private ApiResponse _response;

        public ProductReportService(TkdecorContext context,
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
            var reports = await _context.ProductReports
                .Include(x => x.ProductReported)
                .Include(x => x.UserReport)
                .ToListAsync();

            reports = reports.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<ProductReportGetDto>>(reports);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> Update(Guid id, ProductReportUpdateDto dto)
        {
            if (id != dto.ProductReportId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var report = await _context.ProductReports
                .Include(x => x.ProductReported)
                .Include(x => x.UserReport)
                .FirstOrDefaultAsync(x => x.ProductReportId == id);

            if (report == null || report.IsDelete)
            {
                _response.Message = ErrorContent.ProductReportNotFound;
                return _response;
            }

            report.ReportStatus = dto.ReportStatus;
            report.UpdatedAt = DateTime.Now;

            var message = "";
            if (dto.ReportStatus == SD.ReportAccept)
                message = "được chấp nhận";
            else if (dto.ReportStatus == SD.ReportReject)
                message = "bị từ chối";

            try
            {
                _context.ProductReports.Update(report);

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = report.UserReportId,
                    Message = $"Báo cáo sản phẩm {report.ProductReported.Name} đã {message}."
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
