using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class OrderService : IOrderService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hub;
        private ApiResponse _response;

        public OrderService(TkdecorContext context,
            IMapper mapper,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _hub = hub;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetAll()
        {
            var orders = await _context.Orders
                    .Include(x => x.User)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                    .Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            var result = _mapper.Map<List<OrderGetDto>>(orders);

            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> GetById(Guid id)
        {
            var order = await _context.Orders
                    .Include(x => x.User)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                    .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null || order.IsDelete)
            {
                _response.Message = ErrorContent.OrderNotFound;
            }
            else
            {
                var result = _mapper.Map<OrderGetDto>(order);
                _response.Success = true;
                _response.Data = result;
            }
            return _response;
        }

        public async Task<ApiResponse> UpdateStatusOrder(Guid id, OrderUpdateStatusDto dto)
        {
            if (id != dto.OrderId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var order = await _context.Orders
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null || order.IsDelete)
            {
                _response.Message = ErrorContent.OrderNotFound;
                return _response;
            }

            // update order status
            // customer can cancel, refund, receive the order
            // Admin or seller can confirm the order for delivery
            if (order.OrderStatus == SD.OrderOrdered)
            {
                if (dto.OrderStatus != SD.OrderDelivering
                    && dto.OrderStatus != SD.OrderCanceled)
                {
                    _response.Message = ErrorContent.OrderStatusUnable;
                    return _response;
                }
            }
            else
            {
                _response.Message = ErrorContent.OrderStatusUnable;
                return _response;
            }


            order.OrderStatus = dto.OrderStatus;
            order.UpdatedAt = DateTime.Now;
            try
            {

                var message = "";
                if (dto.OrderStatus == SD.OrderDelivering)
                    message = "được xác nhận";
                else if (dto.OrderStatus == SD.OrderCanceled)
                {
                    message = "bị huỷ";
                    // add quantity of product in store
                    foreach (var orDetail in order.OrderDetails)
                    {
                        orDetail.Product.Quantity += orDetail.Quantity;
                        orDetail.Product.UpdatedAt = DateTime.Now;
                    }
                    message = "huỷ";
                }
                _context.Orders.Update(order);

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = order.UserId,
                    Message = $"Đơn hàng của bạn đã {message}. Mã đơn hàng: " + order.OrderId
                };
                _context.Notifications.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(order.UserId.ToString())
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
