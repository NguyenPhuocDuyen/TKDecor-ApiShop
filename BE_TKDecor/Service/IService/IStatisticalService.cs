using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IStatisticalService
    {
        Task<ApiResponse> GetTotalUser();
        Task<ApiResponse> GetTotalRevenue();
        Task<ApiResponse> GetTotalOrder();
        Task<ApiResponse> RecentOrders();
        Task<ApiResponse> GetTopProductSale(DateTime? startDate, DateTime? endDate, int take);
        Task<ApiResponse> GetRevenueChart(int? year);
    }
}
