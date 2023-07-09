using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAll();
        Task<List<Order>> FindByUserId(Guid userId);
        Task<Order?> FindById(Guid id);
        Task Add(Order order);
        Task Update(Order order);
    }
}
