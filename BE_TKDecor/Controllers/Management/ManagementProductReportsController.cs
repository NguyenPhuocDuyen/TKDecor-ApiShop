using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementProductReportsController : ControllerBase
    {
        private readonly IProductReportService _productReport;

        public ManagementProductReportsController(IProductReportService productReport)
        {
            _productReport = productReport;
        }

        // GET: api/ManagementProductReports/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _productReport.GetAll();
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // PUT: api/ManagementProductReports/UpdateStatusReport/1
        [HttpPut("UpdateStatusReport/{id}")]
        public async Task<IActionResult> UpdateStatusReport(Guid id, ProductReportUpdateDto reportDto)
        {
            var res = await _productReport.Update(id, reportDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
