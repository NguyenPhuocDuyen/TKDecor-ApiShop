using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BE_TKDecor.Core.Dtos.ProductReview;
using BE_TKDecor.Core.Response;
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
        private readonly IUserService _user;

        public ProductReviewsController(IProductReviewService productReview,
            IUserService user)
        {
            _productReview = productReview;
            _user = user;
        }

        // POST: api/ProductReviews/Review
        [HttpPost("Review")]
        public async Task<IActionResult> ReviewProduct(ProductReviewCreateDto productReviewDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _productReview.ReviewProduct(user.UserId, productReviewDto);
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
