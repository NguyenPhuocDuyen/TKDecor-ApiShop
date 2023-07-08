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
        Task<List<Cart>> FindCartsByUserId(Guid userId);
        Task<Cart?> FindByUserIdAndProductId(Guid userId, Guid productId);
        Task<Cart?> FindById(Guid id);
        Task Add(Cart cart);
        Task Update(Cart cart);
    }
}
