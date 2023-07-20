using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class ProductReportRepository : IProductReportRepository
    {
        public async Task Add(ProductReport productReport)
            => await ProductReportDAO.Add(productReport);

        public async Task<ProductReport?> FindById(Guid id)
            => await ProductReportDAO.FindById(id);

        public async Task<ProductReport?> FindByUserIdAndProductId(Guid userId, Guid productId)
            => await ProductReportDAO.FindByUserIdAndProductId(userId, productId);

        public async Task<List<ProductReport>> GetAll()
            => await ProductReportDAO.GetAll();

        public async Task Update(ProductReport productReport)
            => await ProductReportDAO.Update(productReport);
    }
}
