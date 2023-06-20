using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CouponDAO
    {
        public static async Task<List<Coupon>> GetAll()
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

        public static async Task<Coupon?> FindById(int id)
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

        public static async Task<Coupon?> FindByCode(string code)
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

        public static async Task Add(Coupon coupon)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(coupon);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task Update(Coupon coupon)
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
