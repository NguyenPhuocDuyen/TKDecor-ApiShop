using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class ProductFavoriteRepository : IProductFavoriteRepository
    {
        public async Task<List<ProductFavorite>> FindByUserId(Guid userId)
            => await ProductFavoriteDAO.FindByUserId(userId);

        public async Task<ProductFavorite?> FindByUserIdAndProductId(Guid userId, Guid productId)
            => await ProductFavoriteDAO.FindByUserIdAndProductId(userId, productId);

        public async Task Update(ProductFavorite productFavorite)
            => await ProductFavoriteDAO.Update(productFavorite);

        public async Task Add(ProductFavorite productFavorite)
            => await ProductFavoriteDAO.Add(productFavorite);

        public Task<List<ProductFavorite>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ProductFavorite?> FindById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
