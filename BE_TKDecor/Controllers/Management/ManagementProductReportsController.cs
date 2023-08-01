using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementProductReportsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductReportRepository _productReport;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;

        public ManagementProductReportsController(IMapper mapper,
            IProductReportRepository productReport,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub
            )
        {
            _mapper = mapper;
            _productReport = productReport;
            _notification = notification;
            _hub = hub;
        }


        // GET: api/ManagementProductReports/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _productReport.GetAll();
            reports = reports.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<ProductReportGetDto>>(reports);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/ManagementProductReports/UpdateStatusReport
        [HttpPut("UpdateStatusReport/{id}")]
        public async Task<IActionResult> UpdateStatusReport(Guid id, ProductReportUpdateDto reportDto)
        {
            if (id != reportDto.ProductReportId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var report = await _productReport.FindById(id);
            if (report == null || report.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReportNotFound });

            report.ReportStatus = reportDto.ReportStatus;
            report.UpdatedAt = DateTime.Now;

            var message = "";
            if (reportDto.ReportStatus == SD.ReportAccept)
                message = "được chấp nhận";
            else if (reportDto.ReportStatus == SD.ReportReject)
                message = "bị từ chối";

            try
            {
                await _productReport.Update(report);

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = report.UserReportId,
                    Message = $"Báo cáo sản phẩm {report.ProductReported.Name} đã {message}."
                };
                await _notification.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(report.UserReportId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

    }
}
