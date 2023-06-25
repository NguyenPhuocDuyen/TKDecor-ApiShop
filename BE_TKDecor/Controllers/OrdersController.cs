using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using BE_TKDecor.Core.Dtos;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using DataAccess.StatusContent;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
