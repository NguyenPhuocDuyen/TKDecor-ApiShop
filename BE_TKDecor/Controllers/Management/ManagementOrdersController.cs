using AutoMapper;
using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Response;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class ManagementOrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _order;
        private readonly IOrderStatusRepository _orderStatus;

        public ManagementOrdersController(IMapper mapper,
            IOrderRepository order,
            IOrderStatusRepository orderStatus)
        {
            _mapper = mapper;
            _order = order;
            _orderStatus = orderStatus;
        }

        // POST: api/ManagementOrders/GetOrder
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _order.GetAll();
            var result = _mapper.Map<List<OrderGetDto>>(orders);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/ManagementOrders/FindById/1
        [HttpGet("FindById/{id}")]
        public async Task<IActionResult> FindById(Guid id)
        {
            var orders = await _order.FindById(id);
            if (orders == null)
                return NotFound(new ApiResponse { Message = ErrorContent.OrderNotFound });

            var result = _mapper.Map<OrderGetDto>(orders);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/ManagementOrders/UpdateStatusOrder
        [HttpPut("UpdateStatusOrder/{id}")]
        public async Task<IActionResult> UpdateStatusOrder(Guid id, OrderUpdateStatusDto orderUpdateStatusDto)
        {
            if (id != orderUpdateStatusDto.OrderId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var order = await _order.FindById(id);
            if (order == null)
                return NotFound(new ApiResponse { Message = ErrorContent.OrderNotFound });

            if (order.OrderStatus.Name == orderUpdateStatusDto.OrderStatusName)
                return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusUnable });

            // list order status
            var orderStatus = await _orderStatus.GetAll();

            // check order status exists
            var newOrderStatus = orderStatus.FirstOrDefault(x => x.Name == orderUpdateStatusDto.OrderStatusName);
            if (newOrderStatus == null)
                return NotFound(new ApiResponse { Message = ErrorContent.OrderStatusNotFound });

            // update order status
            // customer can cancel, refund, receive the order
            // Admin or seller can confirm the order for delivery
            if (order.OrderStatus.Name == OrderStatusContent.Ordered)
            {
                if (orderUpdateStatusDto.OrderStatusName != OrderStatusContent.Delivering
                    && orderUpdateStatusDto.OrderStatusName != OrderStatusContent.Canceled)
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
                await _order.Update(order);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
