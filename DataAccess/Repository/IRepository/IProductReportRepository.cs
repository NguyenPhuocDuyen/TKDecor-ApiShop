using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductReportRepository : IRepository<ProductReport>
    {
        Task<ProductReport?> FindByUserIdAndProductId(Guid userId, Guid productId);
    }
}
