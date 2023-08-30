using BE_TKDecor.Core.Dtos.ReportProductReview;
using BE_TKDecor.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementReportProductReviewsController : ControllerBase
    {
        private readonly IReportProductReviewService _reportProductReview;

        public ManagementReportProductReviewsController(IReportProductReviewService reportProductReview)
        {
            _reportProductReview = reportProductReview;
        }

        // GET: api/ManagementReportProductReviews/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _reportProductReview.GetAll();
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // PUT: api/ManagementReportProductReviews/UpdateStatusReport/1
        [HttpPut("UpdateStatusReport/{id}")]
        public async Task<IActionResult> UpdateStatusReport(Guid id, ReportProductReviewUpdateDto dto)
        {
            var res = await _reportProductReview.Update(id, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
