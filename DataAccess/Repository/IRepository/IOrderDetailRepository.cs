using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail?> FindByUserIdAndProductId(Guid userId, Guid productId);
    }
}
