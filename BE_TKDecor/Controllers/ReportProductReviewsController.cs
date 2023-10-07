using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Dtos.ReportProductReview;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportProductReviewsController : ControllerBase
    {
        private readonly IReportProductReviewService _reportProductReview;

        public ReportProductReviewsController(IReportProductReviewService reportProductReview)
        {
            _reportProductReview = reportProductReview;
        }

        // POST: api/ReportProductReviews/MakeReport
        [HttpPost("MakeReport")]
        public async Task<IActionResult> MakeReportProductReview(ReportProductReviewCreateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _reportProductReview.MakeReportProductReview(userId, dto);
            if(res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
