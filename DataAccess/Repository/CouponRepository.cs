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
    public class CouponRepository : ICouponRepository
    {
        public async Task Add(Coupon coupon)
            => await CouponDAO.Add(coupon);

        public async Task<Coupon?> FindByCode(string code)
            => await CouponDAO.FindByCode(code);

        public async Task<Coupon?> FindById(Guid id)
            => await CouponDAO.FindById(id);

        public async Task<List<Coupon>> GetAll()
            => await CouponDAO.GetAll();

        public async Task Update(Coupon coupon)
            => await CouponDAO.Update(coupon);
    }
}
