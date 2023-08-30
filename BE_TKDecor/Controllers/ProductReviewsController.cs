using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Dtos.ProductReview;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IProductReviewService _productReview;

        public ProductReviewsController(IProductReviewService productReview)
        {
            _productReview = productReview;
        }

        // POST: api/ProductReviews/Review
        [HttpPost("Review")]
        public async Task<IActionResult> ReviewProduct(ProductReviewCreateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _productReview.ReviewProduct(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
