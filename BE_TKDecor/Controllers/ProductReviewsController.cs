using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BE_TKDecor.Core.Dtos.ProductReview;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly IProductRepository _product;
        private readonly IProductReviewRepository _productReview;
        private readonly IOrderDetailRepository _orderDetail;

        public ProductReviewsController(IMapper mapper,
            IUserRepository user,
            IProductRepository product,
            IProductReviewRepository productReview,
            IOrderDetailRepository orderDetail)
        {
            _mapper = mapper;
            _user = user;
            _product = product;
            _productReview = productReview;
            _orderDetail = orderDetail;
        }

        // POST: api/ProductReviews/Review
        [HttpPost("Review")]
        public async Task<ActionResult<ProductReview>> ReviewProductReview(ProductReviewCreateDto productReviewDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var product = await _product.FindById(productReviewDto.ProductId);
            if (product == null || product.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var orderDetail = await _orderDetail.FindByUserIdAndProductId(user.UserId, product.ProductId);
            if (orderDetail == null)
                return BadRequest(new ApiResponse { Message = "You are not allowed to review the product without buying it!" });

            var productReview = await _productReview.FindByUserIdAndProductId(user.UserId, product.ProductId);

            bool isAdd = true;

            if (productReview == null)
            {
                productReview = new ProductReview()
                {
                    UserId = user.UserId,
                    User = user,
                    ProductId = product.ProductId,
                    Product = product,
                };
            }
            else
            {
                isAdd = false;
                productReview.IsDelete = false;
                productReview.UpdatedAt = DateTime.Now;
            }
            productReview.Rate = productReviewDto.Rate;
            productReview.Description = productReviewDto.Description;

            try
            {
                if (isAdd)
                {
                    await _productReview.Add(productReview);
                }
                else
                {
                    await _productReview.Update(productReview);
                }
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/ProductReviews/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProductReview(Guid id)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var productReview = await _productReview.FindById(id);
            if (productReview == null || productReview.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReviewNotFound });

            productReview.IsDelete = true;
            productReview.UpdatedAt = DateTime.Now;
            try
            {
                await _productReview.Update(productReview);
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
