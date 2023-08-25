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

        public ProductReportService(TkdecorContext context, IMapper mapper, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _hub = hub;
            _response = new ApiResponse();
        }

        // get all report product 
        public async Task<ApiResponse> GetAll()
        {
            var reports = await _context.ProductReports
                .Include(x => x.ProductReported)
                .Include(x => x.UserReport)
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            try
            {
                var result = _mapper.Map<List<ProductReportGetDto>>(reports);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // make report of user
        public async Task<ApiResponse> MakeProductReport(Guid userId, ProductReportCreateDto reportDto)
        {
            var product = await _context.Products.FindAsync(reportDto.ProductReportedId);
            if (product is null || product.IsDelete)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            var report = await _context.ProductReports
                    .FirstOrDefaultAsync(x => x.UserReportId == userId && x.ProductReportedId == product.ProductId);

            bool isAdd = false;
            // create a new report of that user for that product
            // if there is no report that product is in the peding state
            if (report is null || report.ReportStatus != SD.ReportPending)
            {
                isAdd = true;
                report = new ProductReport()
                {
                    UserReportId = userId,
                    ProductReportedId = product.ProductId,
                };
            }
            report.IsDelete = false;
            report.ReportStatus = SD.ReportPending;
            report.Reason = reportDto.Reason;
            report.UpdatedAt = DateTime.Now;

            try
            {
                if (isAdd)
                {
                    _context.ProductReports.Add(report);
                }
                else
                {
                    _context.ProductReports.Update(report);
                }

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = userId,
                    Message = $"Báo cáo sản phẩm {product.Name} thành công"
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

                var user = await _context.Users.FindAsync(userId);
                foreach (var admin in admins)
                {
                    // add notification for user
                    Notification notiForAdmin = new()
                    {
                        UserId = admin.UserId,
                        Message = $"{user?.Email} đã cáo sản phẩm {product.Name}"
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

        // update status of report product
        public async Task<ApiResponse> Update(Guid id, ProductReportUpdateDto dto)
        {
            if (id != dto.ProductReportId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            // get report by id
            var report = await _context.ProductReports
                .Include(x => x.ProductReported)
                .Include(x => x.UserReport)
                .FirstOrDefaultAsync(x => x.ProductReportId == id && !x.IsDelete);

            if (report is null)
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
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
