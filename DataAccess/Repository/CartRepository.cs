using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class CartRepository : ICartRepository
    {
        public async Task Add(Cart cart)
            => await CartDAO.Add(cart);

        public async Task<Cart?> FindById(Guid id)
            => await CartDAO.FindById(id);

        public async Task<Cart?> FindByUserIdAndProductId(Guid userId, Guid productId)
            => await CartDAO.FindByUserIdAndProductId(userId, productId);

        public async Task<List<Cart>> FindCartsByUserId(Guid userId)
            => await CartDAO.FindCartsByUserId(userId);

        public async Task Update(Cart cart)
            => await CartDAO.Update(cart);
    }
}
