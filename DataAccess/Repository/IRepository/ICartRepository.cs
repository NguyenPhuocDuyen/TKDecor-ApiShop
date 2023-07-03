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
        Task<List<Cart>> FindCartsByUserId(long userId);
        Task<Cart?> FindByUserIdAndProductId(long userId, long productId);
        Task<Cart?> FindById(long id);
        Task Add(Cart cart);
        Task Update(Cart cart);
    }
}
