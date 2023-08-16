using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.ProductReview;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hub;
        private ApiResponse _response;

        public ProductReviewService(TkdecorContext context,
            IMapper mapper,
             IHubContext<NotificationHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _hub = hub;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> ReviewProduct(Guid userId, ProductReviewCreateDto productReviewDto)
        {
            var product = await _context.Products.FindAsync(productReviewDto.ProductId);
            if (product == null || product.IsDelete)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            var orderDetail = await _context.OrderDetails.Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Order.UserId == userId && x.ProductId == product.ProductId);

            if (orderDetail == null || orderDetail.Order.OrderStatus != SD.OrderReceived)
            {
                _response.Message = "Bạn không được phép đánh giá nếu chưa mua sản phẩm!";
                return _response;
            }

            var productReview = await _context.ProductReviews
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == product.ProductId);

            bool isAdd = false;

            if (productReview == null)
            {
                isAdd = true;
                productReview = new ProductReview()
                {
                    UserId = userId,
                    ProductId = product.ProductId,
                };
            }
            productReview.IsDelete = false;
            productReview.UpdatedAt = DateTime.Now;
            productReview.Rate = productReviewDto.Rate;
            productReview.Description = productReviewDto.Description;

            try
            {
                if (isAdd)
                {
                    _context.ProductReviews.Add(productReview);
                }
                else
                {
                    _context.ProductReviews.Update(productReview);
                }

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = userId,
                    Message = $"Đã đánh giá sản phẩm {product.Name} thành công"
                };
                _context.Notifications.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(userId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }
    }
}
