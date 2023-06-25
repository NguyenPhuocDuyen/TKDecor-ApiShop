using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface ICartRepository
    {
        Task<List<Cart>> GetCartsByUserId(int userId);
        Task<Cart?> FindByUserIdAndProductId(int userId, int productId);
        Task<Cart?> FindById(int id);
        Task Add(Cart cart);
        Task Update(Cart cart);
        Task Delete(Cart cart);
    }
}
