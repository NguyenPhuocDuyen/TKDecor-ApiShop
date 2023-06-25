using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task Add(Order order);
    }
}
