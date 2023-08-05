using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class ProductImageRepository : IProductImageRepository
    {
        public async Task Delete(ProductImage productImage)
            => await ProductImageDAO.Delete(productImage);
    }
}
