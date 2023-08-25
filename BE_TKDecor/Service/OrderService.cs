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

        public OrderService(TkdecorContext context, IMapper mapper, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _hub = hub;
            _response = new ApiResponse();
        }

        // get all order 
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

            try
            {
                var result = _mapper.Map<List<OrderGetDto>>(orders);

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get all order for 1 user
        public async Task<ApiResponse> GetAllForUser(Guid userId)
        {
            var orders = await _context.Orders
                    .Include(x => x.User)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.ProductReview)
                    .Where(o => o.UserId == userId && !o.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            try
            {
                List<OrderGetDto> result = new();
                // make user revewed
                foreach (var order in orders)
                {
                    var orderGet = _mapper.Map<OrderGetDto>(order);
                    foreach (var od in orderGet.OrderDetails)
                    {
                        od.HasUserReviewed = order.OrderDetails.FirstOrDefault(x => x.OrderDetailId == od.OrderDetailId)?.ProductReview is not null;
                    }
                    result.Add(orderGet);
                }

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get order by id
        public async Task<ApiResponse> GetById(Guid id)
        {
            var order = await GetOrderById(id);

            if (order is null)
            {
                _response.Message = ErrorContent.OrderNotFound;
                return _response;
            }

            try
            {
                var result = _mapper.Map<OrderGetDto>(order);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get order by id user and id order
        public async Task<ApiResponse> GetByIdAndUser(Guid id, Guid userId)
        {
            var order = await GetOrderById(id);

            if (order is null || order.UserId != userId)
            {
                _response.Message = ErrorContent.OrderNotFound;
                return _response;
            }

            try
            {
                var result = _mapper.Map<OrderGetDto>(order);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // make order 
        public async Task<ApiResponse> MakeOrder(User user, OrderMakeDto dto)
        {
            // get coupon
            Coupon? coupon = null;
            if (dto.CodeCoupon is not null)
            {
                coupon = await _context.Coupons
                    .FirstOrDefaultAsync(x => x.Code == dto.CodeCoupon.ToUpper().Trim() && !x.IsDelete);
                if (coupon is null)
                {
                    _response.Message = ErrorContent.CouponNotFound;
                    return _response;
                }

                if (coupon.RemainingUsageCount == 0)
                {
                    _response.Message = "Mã giảm giá hết lượt sử dụng";
                    return _response;
                }

                if (!coupon.IsActive || coupon.StartDate > DateTime.Now || coupon.EndDate < DateTime.Now)
                {
                    _response.Message = "Mã giảm giá không có hiệu lực";
                    return _response;
                }
            }

            // check address
            var address = await _context.UserAddresses.FindAsync(dto.AddressId);
            if (address is null || address.UserId != user.UserId || address.IsDelete)
            {
                _response.Message = ErrorContent.AddressNotFound;
                return _response;
            }

            // get cart of user
            var cartsDb = await _context.Carts
                        .Include(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                        .Where(x => x.UserId == user.UserId && !x.IsDelete).ToListAsync();

            //create new order
            Order newOrder = new()
            {
                UserId = user.UserId,
                OrderStatus = SD.OrderOrdered,
                FullName = address.FullName,
                Phone = address.Phone,
                Address = $"{address.Street}, {address.Ward}, {address.District}, {address.City}",
                Note = dto.Note,
                OrderDetails = new List<OrderDetail>(),
            };

            // add order detail
            foreach (var cartId in dto.ListCartIdSelect)
            {
                // check cart exists
                var cart = cartsDb.FirstOrDefault(x => x.CartId == cartId);
                if (cart is null || cart.IsDelete || cart.UserId != user.UserId)
                {
                    _response.Message = ErrorContent.CartNotFound;
                    return _response;
                }

                if (cart.Quantity > cart.Product.Quantity || cart.Product.Quantity == 0)
                {
                    _response.Message = "Số lượng trong kho không đủ để đặt hàng";
                    return _response;
                }

                OrderDetail orderDetail = new()
                {
                    Order = newOrder,
                    ProductId = cart.ProductId,
                    Quantity = cart.Quantity,
                    PaymentPrice = cart.Product.Price
                };
                newOrder.OrderDetails.Add(orderDetail);
            }

            // calculate total price
            newOrder.TotalPrice = newOrder.OrderDetails.Sum(x => x.PaymentPrice * x.Quantity);

            // check coupon discount
            if (coupon is not null)
            {
                // minus one time using coupon
                coupon.RemainingUsageCount--;

                newOrder.CouponId = coupon.CouponId;
                if (coupon.CouponType == SD.CouponByPercent)
                {
                    // By percent: 100 = 100 - 100 * 0.1 (90)
                    var valueReduce = newOrder.TotalPrice * coupon.Value / 100;
                    newOrder.TotalPrice -= valueReduce < coupon.MaxValue ? valueReduce : coupon.MaxValue;
                    //newOrder.TotalPrice -= newOrder.TotalPrice * coupon.Value / 100;
                }
                else
                {
                    // By value: 100 = 100 - 10 (90)
                    newOrder.TotalPrice -= coupon.Value;
                }

                // If the discount is more than the total amount, you will have to pay $0 
                if (newOrder.TotalPrice < 0)
                {
                    newOrder.TotalPrice = 0;
                }
            }
            // create order and remove item in cart if order success
            try
            {
                // create order
                _context.Orders.Add(newOrder);

                // update coupon
                if (coupon is not null)
                    _context.Coupons.Update(coupon);

                // delete item in cart
                foreach (var cartId in dto.ListCartIdSelect)
                {
                    var cart = cartsDb.FirstOrDefault(x => x.CartId == cartId);
                    if (cart is not null)
                    {
                        // update quantity product in store
                        cart.Product.Quantity -= cart.Quantity;
                        cart.IsDelete = true;
                        _context.Carts.Update(cart);
                    }
                }

                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = "Đặt hàng thành công. Mã đơn hàng: " + newOrder.OrderId
                };
                _context.Notifications.Add(newNotification);
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                // add notification for admin and admin
                var admins = await _context.Users
                    .Where(x => x.Role == SD.RoleAdmin && !x.IsDelete)
                    .ToListAsync();
                foreach (var admin in admins)
                {
                    // add notification for user
                    Notification notiForAdmin = new()
                    {
                        UserId = admin.UserId,
                        Message = $"{user.Email} đã đặt đơn hàng. Mã đơn hàng: {newOrder.OrderId}"
                    };
                    _context.Notifications.Add(notiForAdmin);
                    // notification signalR
                    await _hub.Clients.User(admin.UserId.ToString())
                        .SendAsync(SD.NewNotification,
                        _mapper.Map<NotificationGetDto>(notiForAdmin));
                }
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // update status of order
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
                .FirstOrDefaultAsync(x => x.OrderId == id && !x.IsDelete);

            if (order is null)
            {
                _response.Message = ErrorContent.OrderNotFound;
                return _response;
            }

            // update order status
            // customer can cancel, refund, receive the order
            // Admin can confirm the order for delivery
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
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // update status order for customer
        public async Task<ApiResponse> UpdateStatusOrderForCus(User user, Guid id, OrderUpdateStatusDto dto)
        {
            if (id != dto.OrderId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            // seller and admin have the right to accept orders for delivery
            var order = await _context.Orders
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(x => x.OrderId == id
                                        && !x.IsDelete
                                        && x.UserId == user.UserId);

            if (order is null)
            {
                _response.Message = ErrorContent.OrderNotFound;
                return _response;
            }

            // update order status
            // customer can cancel, refund, receive the order
            //if (order.OrderStatus == SD.OrderOrdered)
            //{
            //    // Cancellation of orders only when the order is in the Ordered status
            //    if (dto.OrderStatus != SD.OrderCanceled)
            //    {
            //        _response.Message = ErrorContent.OrderStatusUnable;
            //        return _response;
            //    }
            //}
            //else if (order.OrderStatus == SD.OrderDelivering)
            //{
            //    // Orders can only be accepted or refunded when the order is in the Delivering status
            //    if (dto.OrderStatus != SD.OrderReceived)
            //    {
            //        _response.Message = ErrorContent.OrderStatusUnable;
            //        return _response;
            //    }
            //}
            //else
            //{
            //    _response.Message = ErrorContent.OrderStatusUnable;
            //    return _response;
            //}
            var message = "";
            // update order status
            // customer can cancel, refund, receive the order
            switch (order.OrderStatus)
            {
                case SD.OrderOrdered:
                    // Cancellation of orders only when the order is in the Ordered status
                    if (dto.OrderStatus != SD.OrderCanceled)
                    {
                        _response.Message = ErrorContent.OrderStatusUnable;
                        return _response;
                    }
                    // add quantity of product in store when canceled
                    foreach (var orDetail in order.OrderDetails)
                    {
                        orDetail.Product.Quantity += orDetail.Quantity;
                        orDetail.Product.UpdatedAt = DateTime.Now;
                    }
                    message = "huỷ";
                    break;
                case SD.OrderDelivering:
                    // Orders can only be accepted or refunded when the order is in the Delivering status
                    if (dto.OrderStatus != SD.OrderReceived)
                    {
                        _response.Message = ErrorContent.OrderStatusUnable;
                        return _response;
                    }
                    message = "nhận";
                    break;
                default:
                    _response.Message = ErrorContent.OrderStatusUnable;
                    return _response;
            }

            order.OrderStatus = dto.OrderStatus;
            order.UpdatedAt = DateTime.Now;
            try
            {
                _context.Orders.Update(order);

                //var message = "";
                //if (dto.OrderStatus == SD.OrderCanceled)
                //{
                //    // add quantity of product in store
                //    foreach (var orDetail in order.OrderDetails)
                //    {
                //        orDetail.Product.Quantity += orDetail.Quantity;
                //        orDetail.Product.UpdatedAt = DateTime.Now;
                //        //await _product.Update(orDetail.Product);
                //    }
                //    message = "huỷ";
                //}
                //else if (dto.OrderStatus == SD.OrderReceived)
                //    message = "nhận";

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Bạn đã {message} đơn hàng thành công. Mã đơn hàng: " + order.OrderId
                };
                _context.Notifications.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                // add notification for admin
                var admins = await _context.Users
                    .Where(x => x.Role == SD.RoleAdmin && !x.IsDelete)
                    .ToListAsync();

                foreach (var admin in admins)
                {
                    // add notification for user
                    Notification notiForAdmin = new()
                    {
                        UserId = admin.UserId,
                        Message = $"{user.Email} đã {message} đơn hàng. Mã đơn hàng: {order.OrderId}"
                    };
                    _context.Notifications.Add(notiForAdmin);
                    // notification signalR
                    await _hub.Clients.User(admin.UserId.ToString())
                        .SendAsync(SD.NewNotification,
                        _mapper.Map<NotificationGetDto>(notiForAdmin));
                }
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        private async Task<Order?> GetOrderById(Guid id)
        {
            return await _context.Orders
                    .Include(x => x.User)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                    .FirstOrDefaultAsync(x => x.OrderId == id && !x.IsDelete);
        }
    }
}
