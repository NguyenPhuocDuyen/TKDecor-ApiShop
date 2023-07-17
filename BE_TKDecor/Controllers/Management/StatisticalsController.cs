using BE_TKDecor.Core.Response;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class StatisticalsController : ControllerBase
    {
        private readonly IUserRepository _user;
        private readonly IOrderRepository _order;

        public StatisticalsController(IUserRepository user,
            IOrderRepository order)
        {
            _user = user;
            _order = order;
        }

        // GET: api/Statisticals/GetTotalUser
        [HttpGet("GetTotalUser")]
        public async Task<IActionResult> GetTotalUser()
        {
            var users = await _user.GetAll();
            return Ok(new ApiResponse { Success = true , Data = users.Count });
        }

        // GET: api/Statisticals/GetTotalRevenue
        [HttpGet("GetTotalRevenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var orders = await _order.GetAll();
            orders = orders.Where(x => x.OrderStatus == OrderStatus.Received).ToList();
            decimal totalRevenue = 0;
            foreach (var o in orders)
            {
                foreach (var item in o.OrderDetails)
                {
                    totalRevenue += item.Quantity * item.PaymentPrice;
                }
            }

            return Ok(new ApiResponse { Success = true, Data = totalRevenue });
        }

        // GET: api/Statisticals/GetTotalOrder
        [HttpGet("GetTotalOrder")]
        public async Task<IActionResult> GetTotalOrder()
        {
            var orders = await _order.GetAll();
            return Ok(new ApiResponse { Success = true, Data = orders.Count });
        }

        // GET: api/Statisticals/GetTotalReturns
        [HttpGet("GetTotalReturns")]
        public async Task<IActionResult> GetTotalReturns()
        {
            var orders = await _order.GetAll();
            orders = orders.Where(x => x.OrderStatus == OrderStatus.Refund).ToList();
            return Ok(new ApiResponse { Success = true, Data = orders.Count });
        }

        // GET: api/Statisticals/GetTopProductSele
        [HttpGet("GetTopProductSele")]
        public async Task<IActionResult> GetTopProductSele(
            DateTime? startDate = null, 
            DateTime? endDate = null,
            int take = 5)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var orders = await _order.GetAll();
            orders = orders.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();

            var productQuantities = orders
                .SelectMany(order => order.OrderDetails) // get all orderDetails from orders
                .GroupBy(orderDetail => orderDetail.ProductId) // group by ProductId
                .Select(group => new
                {
                    ProductId = group.Key,
                    ProductName = group.First().Product.Name,
                    TotalQuantity = group.Sum(orderDetail => orderDetail.Quantity),
                    //Product = group.First().Product
                })
                .OrderByDescending(item => item.TotalQuantity)
                .ToList();

            productQuantities = productQuantities.Take(take).ToList();

            return Ok(new ApiResponse { Success = true, Data = productQuantities });
        }
    }
}
