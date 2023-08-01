using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BE_TKDecor.Core.Dtos.ProductReview;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using Microsoft.AspNetCore.SignalR;
using BE_TKDecor.Hubs;
using Utility;

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
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;

        public ProductReviewsController(IMapper mapper,
            IUserRepository user,
            IProductRepository product,
            IProductReviewRepository productReview,
            IOrderDetailRepository orderDetail,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub
            )
        {
            _mapper = mapper;
            _user = user;
            _product = product;
            _productReview = productReview;
            _orderDetail = orderDetail;
            _notification = notification;
            _hub = hub;
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
            if (orderDetail == null || orderDetail.Order.OrderStatus != SD.OrderReceived)
                return BadRequest(new ApiResponse { Message = "Bạn không được phép đánh giá nếu chưa mua sản phẩm!" });

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

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Đã đánh giá sản phẩm {product.Name} thành công"
                };
                await _notification.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

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
