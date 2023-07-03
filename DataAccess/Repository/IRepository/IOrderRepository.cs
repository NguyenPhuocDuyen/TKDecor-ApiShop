using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAll();
        Task<List<Order>> FindByUserId(long userId);
        Task<Order?> FindById(long id);
        Task Add(Order order);
        Task Update(Order order);
    }
}
