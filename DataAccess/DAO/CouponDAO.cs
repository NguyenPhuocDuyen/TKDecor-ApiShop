using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class CouponDAO : DAO<Coupon>
    {
        internal static async Task<List<Coupon>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var coupons = await context.Coupons
                    .ToListAsync();
                return coupons;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Coupon?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                var coupon = await context.Coupons
                    .FirstOrDefaultAsync(x => x.CouponId == id);
                return coupon;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Coupon?> FindByCode(string code)
        {
            try
            {
                using var context = new TkdecorContext();
                var coupon = await context.Coupons
                    .FirstOrDefaultAsync(x => x.Code == code);
                return coupon;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
