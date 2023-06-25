using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        public async Task<List<OrderStatus>> GetAll()
            => await OrderStatusDAO.GetAll();
        public async Task<OrderStatus?> FindByName(string name)
            => await OrderStatusDAO.FindByName(name);
    }
}
