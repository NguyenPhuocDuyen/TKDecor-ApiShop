using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public async Task Add(Order order)
            => await OrderDAO.Add(order);

        public async Task Update(Order order)
            => await OrderDAO.Update(order);

        public async Task<Order?> FindById(Guid id)
            => await OrderDAO.FindById(id);

        public async Task<List<Order>> FindByUserId(Guid userId)
            => await OrderDAO.FindByUserId(userId);

        public async Task<List<Order>> GetAll()
            => await OrderDAO.GetAll();
    }
}
