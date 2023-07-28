using AutoMapper;
using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Response;
using BusinessObject;
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
        private readonly IMapper _mapper;

        public StatisticalsController(IUserRepository user,
            IOrderRepository order,
            IMapper mapper)
        {
            _user = user;
            _order = order;
            _mapper = mapper;
        }

        // GET: api/Statisticals/GetTotalUser
        [HttpGet("GetTotalUser")]
        public async Task<IActionResult> GetTotalUser()
        {
            var currentDate = DateTime.Now;
            var users = await _user.GetAll();
            users = users.Where(x => !x.IsDelete).ToList();

            // Current month's total users
            int totalUsersCurrentMonth = users.Count(x => x.CreatedAt.Month == currentDate.Month && x.CreatedAt.Year == currentDate.Year);

            // Total users last month
            DateTime lastMonthDate = currentDate.AddMonths(-1);
            int totalUsersPreviousMonth = users.Count(x => x.CreatedAt.Month == lastMonthDate.Month && x.CreatedAt.Year == lastMonthDate.Year);

            // Calculate the ratio of the difference between the total number of users between the current month and the previous month
            double percentageChange = 0;
            if (totalUsersPreviousMonth != 0)
            {
                percentageChange = (double)(totalUsersCurrentMonth - totalUsersPreviousMonth) / totalUsersPreviousMonth;
            }

            var result = new
            {
                TotalUser = users.Count,
                PercentageChange = Math.Round(percentageChange, 3)
            };

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Statisticals/GetTotalRevenue
        [HttpGet("GetTotalRevenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var currentDate = DateTime.Now;
            var orders = await _order.GetAll();
            orders = orders.Where(x => x.OrderStatus == OrderStatus.Received && !x.IsDelete)
                           .ToList();

            // Calculate current month's total revenue
            decimal totalRevenueThisMonth = orders.Where(x => x.CreatedAt.Month == currentDate.Month && x.CreatedAt.Year == currentDate.Year)
                                                  .Sum(x => x.TotalPrice);

            // Calculating total revenue last month
            DateTime lastMonthDate = currentDate.AddMonths(-1);
            decimal totalRevenueLastMonth = orders.Where(x => x.CreatedAt.Month == lastMonthDate.Month && x.CreatedAt.Year == lastMonthDate.Year)
                                                  .Sum(x => x.TotalPrice);

            // Calculate the ratio of sales difference between the current month and the previous month
            decimal revenueDifference = totalRevenueThisMonth - totalRevenueLastMonth;
            double percentageChange = 0;
            if (totalRevenueLastMonth != 0)
            {
                percentageChange = (double)(revenueDifference / totalRevenueLastMonth);
            }

            var result = new
            {
                TotalRevenue = orders.Sum(x => x.TotalPrice),
                PercentageChange = Math.Round(percentageChange, 3)
            };

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Statisticals/GetTotalOrder
        [HttpGet("GetTotalOrder")]
        public async Task<IActionResult> GetTotalOrder()
        {
            var currentDate = DateTime.Now;
            var orders = await _order.GetAll();
            orders = orders.Where(x => !x.IsDelete).ToList();

            // Current total monthly order
            int totalOrdersCurrentMonth = orders.Count(x => x.CreatedAt.Month == currentDate.Month && x.CreatedAt.Year == currentDate.Year);

            // Total orders last month
            DateTime lastMonthDate = currentDate.AddMonths(-1);
            int totalOrdersPreviousMonth = orders.Count(x => x.CreatedAt.Month == lastMonthDate.Month && x.CreatedAt.Year == lastMonthDate.Year);

            // Calculate the ratio of the total order difference between the current month and the previous month
            double percentageChange = 0;
            if (totalOrdersPreviousMonth != 0)
            {
                percentageChange = (double)(totalOrdersCurrentMonth - totalOrdersPreviousMonth) / totalOrdersPreviousMonth;
            }

            var result = new
            {
                TotalOrder = orders.Count,
                PercentageChange = Math.Round(percentageChange, 3)
            };

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Statisticals/RecentOrders
        [HttpGet("RecentOrders")]
        public async Task<IActionResult> RecentOrders()
        {
            var orders = await _order.GetAll();
            orders = orders.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToList();

            var result = _mapper.Map<List<OrderGetDto>>(orders);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        [HttpGet("GetTotalReturns")]
        public async Task<IActionResult> GetTotalReturns()
        {
            var currentDate = DateTime.Now;
            var orders = await _order.GetAll();
            orders = orders.Where(x => x.OrderStatus == OrderStatus.Refund && !x.IsDelete).ToList();

            // Total return orders for the current month
            int totalReturnsCurrentMonth = orders.Count(x => x.CreatedAt.Month == currentDate.Month && x.CreatedAt.Year == currentDate.Year);

            // Total orders returned last month
            DateTime lastMonthDate = currentDate.AddMonths(-1);
            int totalReturnsPreviousMonth = orders.Count(x => x.CreatedAt.Month == lastMonthDate.Month && x.CreatedAt.Year == lastMonthDate.Year);

            // Calculate the ratio of the difference between the total order returns between the current month and the previous month
            double percentageChange = 0;
            if (totalReturnsPreviousMonth != 0)
            {
                percentageChange = (double)(totalReturnsCurrentMonth - totalReturnsPreviousMonth) / totalReturnsPreviousMonth;
            }

            var result = new
            {
                TotalOrderReturn = orders.Count,
                PercentageChange = Math.Round(percentageChange, 3)
            };

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Statisticals/GetTopProductSale
        [HttpGet("GetTopProductSale")]
        public async Task<IActionResult> GetTopProductSale(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int take = 5)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var orders = await _order.GetAll();
            orders = orders.Where(x => !x.IsDelete && x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();

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

        // GET: api/Statisticals/GetRevenueChart
        [HttpGet("GetRevenueChart")]
        public async Task<IActionResult> GetRevenueChart(int? year)
        {
            if (!year.HasValue)
                year = DateTime.Now.Year;

            var orders = await _order.GetAll();
            orders = orders.Where(x => x.OrderStatus == OrderStatus.Received && !x.IsDelete).ToList();
            // Filter orders by the specified year
            orders = orders.Where(x => x.CreatedAt.Year == year).ToList();

            // Group orders by month and calculate the revenue for each month
            var revenueByMonth = orders
                .GroupBy(x => x.CreatedAt.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    Revenue = group.Sum(x => x.TotalPrice)
                })
                .OrderBy(x => x.Month)
                .ToList();

            // Create a dictionary to store the results
            var revenueData = new Dictionary<int, decimal>();

            // Fill the dictionary with revenue data for each month
            for (int month = 1; month <= 12; month++)
            {
                var revenue = revenueByMonth.FirstOrDefault(x => x.Month == month);
                revenueData.Add(month, revenue?.Revenue ?? 0);
            }

            return Ok(new ApiResponse { Success = true, Data = revenueData });
        }
    }
}
