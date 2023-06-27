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
    public class ProductReportRepository : IProductReportRepository
    {
        public async Task Add(ProductReport productReport)
            => await ProductReportDAO.Add(productReport);

        public async Task<ProductReport?> FindById(int id)
            => await ProductReportDAO.FindById(id);

        public async Task<ProductReport?> FindByUserIdAndProductId(int userId, int productId)
            => await ProductReportDAO.FindByUserIdAndProductId(userId, productId);

        public async Task<List<ProductReport>> GetAll()
            => await ProductReportDAO.GetAll();

        public async Task Update(ProductReport productReport)
            => await ProductReportDAO.Update(productReport);
    }
}
