using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BE_TKDecor.Core.Dtos.ProductReport;
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

        public ProductReportsController(IProductReportService productReport)
        {
            _productReport = productReport;
        }

        // POST: api/ProductReports/MakeProductReport
        [HttpPost("MakeProductReport")]
        public async Task<IActionResult> MakeProductReport(ProductReportCreateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _productReport.MakeProductReport(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
