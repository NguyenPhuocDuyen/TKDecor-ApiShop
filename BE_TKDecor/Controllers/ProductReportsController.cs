using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using Utility;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleCustomer)]
    public class ProductReportsController : ControllerBase
    {
        private readonly IProductReportService _productReport;
        private readonly IUserService _user;

        public ProductReportsController(IProductReportService productReport,
            IUserService user)
        {
            _productReport = productReport;
            _user = user;
        }

        // POST: api/ProductReports/MakeProductReport
        [HttpPost("MakeProductReport")]
        public async Task<ActionResult<ProductReport>> MakeProductReport(ProductReportCreateDto reportDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _productReport.MakeProductReport(user.UserId, reportDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.GetById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
