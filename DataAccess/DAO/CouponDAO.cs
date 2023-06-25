using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class CouponDAO
    {
        internal static async Task<List<Coupon>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var coupons = await context.Coupons
                    .Include(x => x.CouponType)
                    .ToListAsync();
                return coupons;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Coupon?> FindById(int id)
        {
            try
            {
                using var context = new TkdecorContext();
                var coupon = await context.Coupons
                    .Include(x => x.CouponType)
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
                    .Include(x => x.CouponType)
                    .FirstOrDefaultAsync(x => x.Code == code);
                return coupon;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Add(Coupon coupon)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(coupon);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(Coupon coupon)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(coupon);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
