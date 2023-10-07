using Microsoft.AspNetCore.Mvc;
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

        public OrdersController(IOrderService order)
        {
            _order = order;
        }

        // GET: api/Orders/GetAllOfUser
        [HttpGet("GetAllOfUser")]
        public async Task<IActionResult> GetAll()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _order.GetAllForUser(userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Orders/MakeOrder
        [HttpPost("MakeOrder")]
        public async Task<IActionResult> MakeOrder(OrderMakeDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _order.MakeOrder(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Orders/UpdateStatusOrder/1
        [HttpPut("UpdateStatusOrder/{id}")]
        public async Task<IActionResult> UpdateStatusOrder(Guid id, OrderUpdateStatusDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _order.UpdateStatusOrderForCus(userId, id, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
