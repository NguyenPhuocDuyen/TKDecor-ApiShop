using AutoMapper;
using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class StatisticalService : IStatisticalService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public StatisticalService(TkdecorContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetRevenueChart(int? year)
        {
            if (!year.HasValue)
                year = DateTime.Now.Year;

            // Filter orders by the specified year
            var orders = await _context.Orders
                        .Where(x => x.OrderStatus == SD.OrderReceived && !x.IsDelete
                        && x.CreatedAt.Year == year).ToListAsync();

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

            _response.Success = true;
            _response.Data = revenueData;
            return _response;
        }

        public async Task<ApiResponse> GetTopProductSale(DateTime? startDate, DateTime? endDate, int take)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var orders = await _context.Orders
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .Where(x => !x.IsDelete && x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .ToListAsync();

            var productQuantities = orders
                .SelectMany(order => order.OrderDetails) // get all orderDetails from orders
                .GroupBy(orderDetail => orderDetail.ProductId) // group by ProductId
                .Select(group => new
                {
                    ProductId = group.Key,
                    ProductName = group.First().Product.Name,
                    TotalQuantity = group.Sum(orderDetail => orderDetail.Quantity),
                })
                .OrderByDescending(item => item.TotalQuantity)
                .ToList();

            productQuantities = productQuantities.Take(take).ToList();

            _response.Success = true;
            _response.Data = productQuantities;
            return _response;
        }

        public async Task<ApiResponse> GetTotalOrder()
        {
            var currentDate = DateTime.Now;
            var orders = await _context.Orders.Where(x => !x.IsDelete).ToListAsync();

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

            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> GetTotalRevenue()
        {
            var currentDate = DateTime.Now;
            var orders = await _context.Orders
                        //.Include(x => x.User)
                        //.Include(x => x.OrderDetails)
                        //    .ThenInclude(x => x.Product)
                        //        .ThenInclude(x => x.ProductImages)
                        .ToListAsync();

            orders = orders.Where(x => x.OrderStatus == SD.OrderReceived && !x.IsDelete)
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

            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> GetTotalUser()
        {
            var currentDate = DateTime.Now;
            var users = await _context.Users.ToListAsync();
            users = users.Where(x => !x.IsDelete && x.Role == SD.RoleCustomer).ToList();

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

            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> RecentOrders()
        {
            var orders = await _context.Orders.Where(x => !x.IsDelete)
                    .Include(x => x.User)
                    //.Include(x => x.OrderDetails)
                    //    .ThenInclude(x => x.Product)
                    //        .ThenInclude(x => x.ProductImages)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(10)
                    .ToListAsync();

            var result = _mapper.Map<List<OrderGetDto>>(orders);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }
    }
}
