using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<List<Cart>> FindCartsByUserId(Guid userId);
        Task<Cart?> FindByUserIdAndProductId(Guid userId, Guid productId);
    }
}
