using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementOrdersController : ControllerBase
    {
        private readonly IOrderService _order;

        public ManagementOrdersController(IOrderService order)
        {
            _order = order;
        }

        // POST: api/ManagementOrders/GetOrder
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _order.GetAll();
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/ManagementOrders/FindById/1
        [HttpGet("FindById/{id}")]
        public async Task<IActionResult> FindById(Guid id)
        {
            var res = await _order.GetById(id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/ManagementOrders/UpdateStatusOrder
        [HttpPut("UpdateStatusOrder/{id}")]
        public async Task<IActionResult> UpdateStatusOrder(Guid id, OrderUpdateStatusDto orderDto)
        {
            var res = await _order.UpdateStatusOrder(id, orderDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
