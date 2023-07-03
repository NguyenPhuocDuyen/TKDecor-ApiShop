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
        Task<ProductReport?> FindById(long id);
        Task<ProductReport?> FindByUserIdAndProductId(long userId, long productId);
        Task Add(ProductReport productReport);
        Task Update(ProductReport productReport);
    }
}
