using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductReportRepository
    {
        Task<List<ProductReport>> GetAll();
        Task<ProductReport?> FindById(Guid id);
        Task<ProductReport?> FindByUserIdAndProductId(Guid userId, Guid productId);
        Task Add(ProductReport productReport);
        Task Update(ProductReport productReport);
    }
}
