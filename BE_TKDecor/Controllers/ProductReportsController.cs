using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BE_TKDecor.Hubs;
using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using Utility;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleCustomer)]
    public class ProductReportsController : ControllerBase
    {
        private readonly IUserRepository _user;
        private readonly IProductRepository _product;
        private readonly IProductReportRepository _productReport;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMapper _mapper;

        public ProductReportsController(IUserRepository user,
            IProductRepository product,
            IProductReportRepository productReport,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub,
            IMapper mapper
            )
        {
            _user = user;
            _product = product;
            _productReport = productReport;
            _notification = notification;
            _hub = hub;
            _mapper = mapper;
        }

        // POST: api/ProductReports/MakeProductReport
        [HttpPost("MakeProductReport")]
        public async Task<ActionResult<ProductReport>> MakeProductReport(ProductReportCreateDto reportDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var product = await _product.FindById(reportDto.ProductReportedId);
            if (product == null || product.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var report = await _productReport.FindByUserIdAndProductId(user.UserId, product.ProductId);

            bool isAdd = false;
            // create a new report of that user for that product
            // if there is no report that product is in the peding state
            if (report == null)
            {
                isAdd = true;
                report = new ProductReport()
                {
                    UserReportId = user.UserId,
                    UserReport = user,
                    ProductReportedId = product.ProductId,
                    ProductReported = product,
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
                    await _productReport.Add(report);
                }
                else
                {
                    await _productReport.Update(report);
                }

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Báo cáo sản phẩm {product.Name} thành công"
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
                        Message = $"{user.Email} đã cáo sản phẩm {product.Name}"
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
