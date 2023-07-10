using BusinessObject;

namespace DataAccess.Repository.IRepository
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon?> FindByCode(string code);
    }
}
