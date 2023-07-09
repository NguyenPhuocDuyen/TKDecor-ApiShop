using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Customer)]
    public class ProductReportsController : ControllerBase
    {
        private readonly IUserRepository _user;
        private readonly IProductRepository _product;
        private readonly IProductReportRepository _productReport;
        private readonly IReportStatusRepository _reportStatus;

        public ProductReportsController(IUserRepository user,
            IProductRepository product,
            IProductReportRepository productReport,
            IReportStatusRepository reportStatus
            )
        {
            _user = user;
            _product = product;
            _productReport = productReport;
            _reportStatus = reportStatus;
        }

        // POST: api/ProductReports/MakeProductReport
        [HttpPost("MakeProductReport")]
        public async Task<ActionResult<ProductReport>> MakeProductReport(ProductReportCreateDto reportDto)
        {
            var product = await _product.FindById(reportDto.ProductReportedId);
            if (product == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var reportStatus = await _reportStatus.GetAll();
            var statusPeding = reportStatus.FirstOrDefault(x => x.Name == ReportStatusContent.Pending);
            if (statusPeding == null)
                return BadRequest(new ApiResponse { Message = ErrorContent.Error });

            var report = await _productReport.FindByUserIdAndProductId(user.UserId, product.ProductId);

            bool isAdd = true;
            // create a new report of that user for that product
            // if there is no report that product is in the peding state
            if (report == null || (report.ReportStatus.Name != ReportStatusContent.Pending))
            {
                report = new ProductReport()
                {
                    UserReportId = user.UserId,
                    UserReport = user,
                    ProductReportedId = product.ProductId,
                    ProductReported = product,
                    ReportStatusId = statusPeding.ReportStatusId,
                    ReportStatus = statusPeding,
                    Reason = reportDto.Reason
                };
            }
            else
            {
                isAdd = false;
                report.IsDelete = false;
                report.Reason = reportDto.Reason;
                report.UpdatedAt = DateTime.UtcNow;
            }

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
                    return await _user.FindById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
