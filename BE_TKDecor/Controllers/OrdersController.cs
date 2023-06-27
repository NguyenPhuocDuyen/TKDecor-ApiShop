using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using DataAccess.StatusContent;
using BE_TKDecor.Core.Dtos.Order;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Customer)]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly ICartRepository _cartRepository;

        public OrdersController(IMapper mapper,
            IUserRepository userRepository,
            IUserAddressRepository userAddressRepository,
            ICouponRepository couponRepository,
            IOrderRepository orderRepository,
            IOrderStatusRepository orderStatusRepository,
            ICartRepository cartRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userAddressRepository = userAddressRepository;
            _couponRepository = couponRepository;
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
            _cartRepository = cartRepository;
        }

        // POST: api/Orders/GetOrder
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var user = await GetUser();
            if (user == null)
                return BadRequest(new ApiResponse { Message = ErrorContent.UserNotFound });

            var orders = await _orderRepository.GetByUserId(user.UserId);
            var result = _mapper.Map<List<OrderGetDto>>(orders);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/Orders/MakeOrder
        [HttpPost("MakeOrder")]
        public async Task<IActionResult> MakeOrder(OrderMakeDto orderDto)
        {
            var user = await GetUser();
            if (user == null)
                return BadRequest(new ApiResponse { Message = ErrorContent.UserNotFound });

            // get coupon
            Coupon? coupon = null;
            if (orderDto.CodeCoupon != null)
            {
                coupon = await _couponRepository.FindByCode(orderDto.CodeCoupon);
                if (coupon == null)
                    return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });
            }
            // check address
            var address = await _userAddressRepository.FindById(orderDto.AddressId);
            if (address == null || address.UserId != user.UserId)
                return NotFound(new ApiResponse { Message = ErrorContent.AddressNotFound });

            // get  status order ordered
            var orderedStatus = await _orderStatusRepository.FindByName(OrderStatusContent.Ordered);
            if (orderedStatus == null)
                return BadRequest(new ApiResponse { Message = ErrorContent.Error });

            // get cart of user
            var cartsDb = await _cartRepository.GetCartsByUserId(user.UserId);

            //create new order
            Order newOrder = new()
            {
                UserId = user.UserId,
                User = user,
                OrderStatusId = orderedStatus.OrderStatusId,
                OrderStatus = orderedStatus,
                FullName = address.FullName,
                Phone = address.Phone,
                Address = address.Address,
                //TotalPrice
            };
            // add order detail
            foreach (var cartId in orderDto.ListCartIdSelect)
            {
                // check exists
                if (!cartsDb.Select(x => x.CartId).ToList().Contains(cartId))
                    return NotFound(new ApiResponse { Message = ErrorContent.CartNotFound });

                var cart = cartsDb.FirstOrDefault(x => x.CartId == cartId && x.UserId == user.UserId);
                if (cart == null)
                    return NotFound(new ApiResponse { Message = ErrorContent.CartNotFound });

                OrderDetail orderDetail = new()
                {
                    Order = newOrder,
                    ProductId = cart.ProductId,
                    Product = cart.Product,
                    Quantity = cart.Quantity,
                    PaymentPrice = cart.Product.Price
                };
                newOrder.OrderDetails.Add(orderDetail);
            }
            // calculate total price
            newOrder.TotalPrice = newOrder.OrderDetails.Sum(x => x.PaymentPrice);
            // check coupon discount
            if (coupon != null)
            {
                if (coupon.CouponType.Name == CouponTypeContent.ByPercent)
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
                await _orderRepository.Add(newOrder);
                // delete item in cart
                foreach (var cartId in orderDto.ListCartIdSelect)
                {
                    var cart = cartsDb.FirstOrDefault(x => x.CartId == cartId);
                    if (cart != null)
                        await _cartRepository.Delete(cart);
                }
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/Orders/UpdateStatusOrder
        [HttpPut("UpdateStatusOrder/{id}")]
        public async Task<IActionResult> UpdateStatusOrder(int id, OrderUpdateStatusDto orderUpdateStatusDto)
        {
            if (id != orderUpdateStatusDto.OrderId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            // seller and admin have the right to accept orders for delivery
            var order = await _orderRepository.FindById(id);
            if (order == null)
                return NotFound(new ApiResponse { Message = ErrorContent.OrderNotFound });

            if (order.OrderStatus.Name == orderUpdateStatusDto.OrderStatusName)
                return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusUnable });

            // list order status
            var orderStatus = await _orderStatusRepository.GetAll();

            // check order status exists
            var newOrderStatus = orderStatus.FirstOrDefault(x => x.Name == orderUpdateStatusDto.OrderStatusName);
            if (newOrderStatus == null)
                return NotFound(new ApiResponse { Message = ErrorContent.OrderStatusNotFound });

            // update order status
            // customer can cancel, refund, receive the order
            if (order.OrderStatus.Name == OrderStatusContent.Ordered)
            {
                // Cancellation of orders only when the order is in the Ordered status
                if (orderUpdateStatusDto.OrderStatusName != OrderStatusContent.Canceled)
                    return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusUnable });
            }
            else if (order.OrderStatus.Name == OrderStatusContent.Delivering)
            {
                // Orders can only be accepted or refunded when the order is in the Delivering status
                if (orderUpdateStatusDto.OrderStatusName != OrderStatusContent.Received
                    && orderUpdateStatusDto.OrderStatusName != OrderStatusContent.Refund)
                    return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusUnable });
            }
            else
            {
                return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusUnable });
            }

            order.OrderStatus = newOrderStatus;
            order.OrderStatusId = newOrderStatus.OrderStatusId;
            order.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _orderRepository.Update(order);
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
