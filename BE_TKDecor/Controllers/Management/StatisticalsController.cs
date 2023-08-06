using BE_TKDecor.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class StatisticalsController : ControllerBase
    {
        private readonly IStatisticalService _statistical;

        public StatisticalsController(IStatisticalService statistical)
        {
            _statistical = statistical;
        }

        // GET: api/Statisticals/GetTotalUser
        [HttpGet("GetTotalUser")]
        public async Task<IActionResult> GetTotalUser()
        {
            var res = await _statistical.GetTotalUser();
            return Ok(res);
        }

        // GET: api/Statisticals/GetTotalRevenue
        [HttpGet("GetTotalRevenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var res = await _statistical.GetTotalRevenue();
            return Ok(res);
        }

        // GET: api/Statisticals/GetTotalOrder
        [HttpGet("GetTotalOrder")]
        public async Task<IActionResult> GetTotalOrder()
        {
            var res = await _statistical.GetTotalOrder();
            return Ok(res);
        }

        // GET: api/Statisticals/RecentOrders
        [HttpGet("RecentOrders")]
        public async Task<IActionResult> RecentOrders()
        {
            var res = await _statistical.RecentOrders();
            return Ok(res);
        }

        // GET: api/Statisticals/GetTopProductSale
        [HttpGet("GetTopProductSale")]
        public async Task<IActionResult> GetTopProductSale(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int take = 5)
        {
            var res = await _statistical.GetTopProductSale(startDate, endDate, take);
            return Ok(res);
        }

        // GET: api/Statisticals/GetRevenueChart
        [HttpGet("GetRevenueChart")]
        public async Task<IActionResult> GetRevenueChart(int? year)
        {
            var res = await _statistical.GetRevenueChart(year);
            return Ok(res);
        }
    }
}
