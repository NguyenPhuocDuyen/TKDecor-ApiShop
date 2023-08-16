using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Dtos.Order;
using Utility;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleCustomer)]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _order;
        private readonly IUserService _user;

        public OrdersController(IOrderService order, IUserService user)
        {
            _order = order;
            _user = user;
        }

        // GET: api/Orders/GetAllOfUser
        [HttpGet("GetAllOfUser")]
        public async Task<IActionResult> GetAll()
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _order.GetAllForUser(user.UserId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        //// GET: api/Orders/FindById/1
        //[HttpGet("FindById/{id}")]
        //public async Task<IActionResult> FindById(Guid id)
        //{
        //    var user = await GetUser();
        //    if (user == null || user.IsDelete)
        //        return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

        //    var res = await _order.GetByIdAndUser(id, user.UserId);
        //    if (res.Success)
        //    {
        //        return Ok(res);
        //    }
        //    return BadRequest(res);
        //}

        // POST: api/Orders/MakeOrder
        [HttpPost("MakeOrder")]
        public async Task<IActionResult> MakeOrder(OrderMakeDto orderDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _order.MakeOrder(user, orderDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Orders/UpdateStatusOrder
        [HttpPut("UpdateStatusOrder/{id}")]
        public async Task<IActionResult> UpdateStatusOrder(Guid id, OrderUpdateStatusDto orderDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _order.UpdateStatusOrderForCus(user, id, orderDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.GetById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
