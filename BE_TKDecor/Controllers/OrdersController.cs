using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using BE_TKDecor.Core.Dtos;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IMapper mapper,
            IUserRepository userRepository,
            ICouponRepository couponRepository,
            IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _couponRepository = couponRepository;
            _orderRepository = orderRepository;
        }

        //// POST: api/Orders/MakeOrder
        //[HttpPost("MakeOrder")]
        //public async Task<IActionResult> MakeOrder(OrderMakeDto orderDto)
        //{
        //    var user = await GetUser();
        //    if (user == null)
        //        return BadRequest(new ApiResponse { Message = ErrorContent.UserNotFound });

        //    var code = 
        //}

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
