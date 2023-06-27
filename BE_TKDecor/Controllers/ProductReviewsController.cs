using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BE_TKDecor.Core.Dtos.ProductReview;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _productReviewRepository;

        public ProductReviewsController(IUserRepository userRepository,
            IProductRepository productRepository,
            IProductReviewRepository productReviewRepository)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
        }

        // POST: api/ProductReviews/Review
        [HttpPost("Review")]
        public async Task<ActionResult<ProductReview>> ReviewProductReview(ProductReviewCreateDto productReviewDto)
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var product = await _productRepository.FindById(productReviewDto.ProductId);
            if (product == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var canReview = await _productReviewRepository.CanReview(user.UserId, product.ProductId);
            if (!canReview)
                return BadRequest(new ApiResponse { Message = "You are not allowed to review the product without buying it!" });

            var productReview = await _productReviewRepository.GetByUserIdAndProductId(user.UserId, product.ProductId);

            bool isAdd = true;

            if (productReview == null)
            {
                productReview = new ProductReview()
                {
                    UserId = user.UserId,
                    User = user,
                    ProductId = product.ProductId,
                    Product = product,
                    Rate = productReviewDto.Rate,
                    Description = productReviewDto.Description
                };
            }
            else
            {
                isAdd = false;
                productReview.Rate = productReviewDto.Rate;
                productReview.Description = productReviewDto.Description;
                productReview.UpdatedAt = DateTime.UtcNow;
            }

            try
            {
                if (isAdd)
                {
                    await _productReviewRepository.Add(productReview);
                }
                else
                {
                    await _productReviewRepository.Update(productReview);
                }
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/ProductReviews/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProductReview(int id)
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var productReview = await _productReviewRepository.FindById(id);
            if (productReview == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReviewNotFound });

            productReview.IsDelete = true;
            productReview.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _productReviewRepository.Update(productReview);
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
                    return await _userRepository.FindById(int.Parse(userId));
            }
            return null;
        }
    }
}
