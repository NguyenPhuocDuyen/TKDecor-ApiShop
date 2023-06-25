using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        public async Task<OrderStatus?> FindByName(string name)
            => await OrderStatusDAO.FindByName(name);
    }
}
