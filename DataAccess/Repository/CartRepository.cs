using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class CartRepository : ICartRepository
    {
        public async Task Add(Cart cart)
            => await CartDAO.Add(cart);

        public async Task Delete(Cart cart)
            => await CartDAO.Delete(cart);

        public async Task<Cart?> FindById(int id)
            => await CartDAO.FindById(id);

        public async Task<Cart?> FindByUserIdAndProductId(int userId, int productId)
            => await CartDAO.FindByUserIdAndProductId(userId, productId);

        public async Task<List<Cart>> GetCartsByUserId(int userId)
            => await CartDAO.GetCartsByUserId(userId);

        public async Task Update(Cart cart)
            => await CartDAO.Update(cart);
    }
}
