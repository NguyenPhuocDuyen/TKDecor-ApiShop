using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface ICouponRepository
    {
        Task<List<Coupon>> GetAll();
        Task<Coupon?> FindById(Guid id);
        Task<Coupon?> FindByCode(string code);
        Task Add(Coupon coupon);
        Task Update(Coupon coupon);
    }
}
