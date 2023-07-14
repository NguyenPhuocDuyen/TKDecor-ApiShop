using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public async Task<OrderDetail?> FindByUserIdAndProductId(Guid userId, Guid productId)
            => await OrderDetailDAO.FindByUserIdAndProductId(userId, productId);
    }
}
