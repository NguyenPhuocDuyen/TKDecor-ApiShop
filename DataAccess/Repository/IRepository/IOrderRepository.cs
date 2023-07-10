using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> FindByUserId(Guid userId);
    }
}
