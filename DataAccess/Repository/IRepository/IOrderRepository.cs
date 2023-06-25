using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAll();
        Task<List<Order>> GetByUserId(int userId);
        Task<Order?> FindById(int id);
        Task Add(Order order);
        Task Update(Order order);
    }
}
