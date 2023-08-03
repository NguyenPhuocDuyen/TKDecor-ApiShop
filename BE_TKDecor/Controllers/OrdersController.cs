using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Hubs;
using Microsoft.AspNetCore.SignalR;
using BE_TKDecor.Core.Dtos.Notification;
using Utility;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleCustomer)]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly IUserAddressRepository _userAddress;
        private readonly ICouponRepository _coupon;
        private readonly IOrderRepository _order;
        private readonly ICartRepository _cart;
        private readonly IProductRepository _product;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;

        public OrdersController(IMapper mapper,
            IUserRepository user,
            IUserAddressRepository userAddress,
            ICouponRepository coupon,
            IOrderRepository order,
            ICartRepository cart,
            IProductRepository product,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub)
        {
            _mapper = mapper;
            _user = user;
            _userAddress = userAddress;
            _coupon = coupon;
            _order = order;
            _cart = cart;
            _product = product;
            _notification = notification;
            _hub = hub;
        }

        // GET: api/Orders/GetAllOfUser
        [HttpGet("GetAllOfUser")]
        public async Task<IActionResult> GetAll()
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var orders = await _order.FindByUserId(user.UserId);
            orders = orders.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<OrderGetDto>>(orders);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Orders/FindById/1
        [HttpGet("FindById/{id}")]
        public async Task<IActionResult> FindById(Guid id)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var order = await _order.FindById(id);
            if (order == null || order.UserId != user.UserId || order.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.OrderNotFound });

            var result = _mapper.Map<OrderGetDto>(order);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/Orders/MakeOrder
        [HttpPost("MakeOrder")]
        public async Task<IActionResult> MakeOrder(OrderMakeDto orderDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            // get coupon
            Coupon? coupon = null;
            if (!string.IsNullOrEmpty(orderDto.CodeCoupon))
            {
                coupon = await _coupon.FindByCode(orderDto.CodeCoupon);
                if (coupon == null || coupon.IsDelete)
                    return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });

                if (coupon.RemainingUsageCount == 0)
                    return BadRequest(new ApiResponse { Message = "Mã giảm giá hết lượt sử dụng" });

                if (!coupon.IsActive || coupon.StartDate > DateTime.Now || coupon.EndDate < DateTime.Now)
                    return BadRequest(new ApiResponse { Message = "Mã giảm giá không có hiệu lực" });
            }

            // check address
            var address = await _userAddress.FindById(orderDto.AddressId);
            if (address == null || address.UserId != user.UserId)
                return NotFound(new ApiResponse { Message = ErrorContent.AddressNotFound });

            // get cart of user
            var cartsDb = await _cart.FindCartsByUserId(user.UserId);

            //create new order
            Order newOrder = new()
            {
                UserId = user.UserId,
                OrderStatus = SD.OrderOrdered,
                FullName = address.FullName,
                Phone = address.Phone,
                Address = $"{address.Street}, {address.Ward}, {address.District}, {address.City}",
                Note = orderDto.Note,
                OrderDetails = new List<OrderDetail>(),
                //TotalPrice
            };

            // add order detail
            foreach (var cartId in orderDto.ListCartIdSelect)
            {
                // check exists
                if (!cartsDb.Select(x => x.CartId).ToList().Contains(cartId))
                    return NotFound(new ApiResponse { Message = ErrorContent.CartNotFound });

                var cart = cartsDb.FirstOrDefault(x => x.CartId == cartId);
                if (cart == null || cart.IsDelete || cart.UserId != user.UserId)
                    return NotFound(new ApiResponse { Message = ErrorContent.CartNotFound });

                if (cart.Quantity > cart.Product.Quantity)
                    return BadRequest(new ApiResponse { Message = "Số lượng trong kho không đủ để đặt hàng" });

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
            if (coupon != null)
            {
                coupon.RemainingUsageCount--;
                newOrder.CouponId = coupon.CouponId;
                if (coupon.CouponType == SD.CouponByPercent)
                {
                    // By percent: 100 = 100 - 100 * 0.1 (90)
                    newOrder.TotalPrice -= newOrder.TotalPrice * coupon.Value;
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
                await _order.Add(newOrder);

                // update coupon
                if (coupon != null)
                    await _coupon.Update(coupon);

                // delete item in cart
                foreach (var cartId in orderDto.ListCartIdSelect)
                {
                    var cart = cartsDb.FirstOrDefault(x => x.CartId == cartId);
                    if (cart != null)
                    {
                        // update quantity product in store
                        cart.Product.Quantity -= cart.Quantity;
                        cart.IsDelete = true;
                        await _cart.Update(cart);
                    }
                }

                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = "Đặt hàng thành công. Mã đơn hàng: " + newOrder.OrderId
                };
                await _notification.Add(newNotification);
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                // add notification for staff and admin
                var listStaffOrAdmin = (await _user.GetAll()).Where(x => x.Role != SD.RoleCustomer);
                foreach (var staff in listStaffOrAdmin)
                {
                    // add notification for user
                    Notification notiForStaffOrAdmin = new()
                    {
                        UserId = staff.UserId,
                        Message = $"{user.Email} đã đặt đơn hàng. Mã đơn hàng: {newOrder.OrderId}"
                    };
                    await _notification.Add(notiForStaffOrAdmin);
                    // notification signalR
                    await _hub.Clients.User(staff.UserId.ToString())
                        .SendAsync(SD.NewNotification,
                        _mapper.Map<NotificationGetDto>(notiForStaffOrAdmin));
                }

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/Orders/UpdateStatusOrder
        [HttpPut("UpdateStatusOrder/{id}")]
        public async Task<IActionResult> UpdateStatusOrder(Guid id, OrderUpdateStatusDto orderDto)
        {
            if (id != orderDto.OrderId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            // seller and admin have the right to accept orders for delivery
            var order = await _order.FindById(id);
            if (order == null || order.UserId != user.UserId || order.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.OrderNotFound });

            // update order status
            // customer can cancel, refund, receive the order
            if (order.OrderStatus == SD.OrderOrdered)
            {
                // Cancellation of orders only when the order is in the Ordered status
                if (orderDto.OrderStatus != SD.OrderCanceled)
                    return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusUnable });
            }
            else if (order.OrderStatus == SD.OrderDelivering)
            {
                // Orders can only be accepted or refunded when the order is in the Delivering status
                if (orderDto.OrderStatus != SD.OrderReceived)
                    return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusUnable });
            }
            else
            {
                return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusUnable });
            }

            order.OrderStatus = orderDto.OrderStatus;
            order.UpdatedAt = DateTime.Now;
            try
            {
                await _order.Update(order);

                var message = "";
                if (orderDto.OrderStatus == SD.OrderCanceled)
                {
                    // add quantity of product in store
                    foreach (var orDetail in order.OrderDetails)
                    {
                        orDetail.Product.Quantity += orDetail.Quantity;
                        orDetail.Product.UpdatedAt = DateTime.Now;
                        await _product.Update(orDetail.Product);
                    }
                    message = "huỷ";
                }
                else if (orderDto.OrderStatus == SD.OrderReceived)
                    message = "nhận";
                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Bạn đã {message} đơn hàng thành công. Mã đơn hàng: " + order.OrderId
                };
                await _notification.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                // add notification for staff and admin
                var listStaffOrAdmin = (await _user.GetAll()).Where(x => x.Role != SD.RoleCustomer);
                foreach (var staff in listStaffOrAdmin)
                {
                    // add notification for user
                    Notification notiForStaffOrAdmin = new()
                    {
                        UserId = staff.UserId,
                        Message = $"{user.Email} đã {message} đơn hàng. Mã đơn hàng: {order.OrderId}"
                    };
                    await _notification.Add(notiForStaffOrAdmin);
                    // notification signalR
                    await _hub.Clients.User(staff.UserId.ToString())
                        .SendAsync(SD.NewNotification,
                        _mapper.Map<NotificationGetDto>(notiForStaffOrAdmin));
                }

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
