using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICouponRepository _coupon;

        public CouponsController(IMapper mapper,
            ICouponRepository coupon)
        {
            _mapper = mapper;
            _coupon = coupon;
        }

        //// GET: api/Coupons/GetALl
        //[HttpGet("GetAll")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var coupons = (await _coupon.GetAll())
        //            .Where(x => x.IsActive == true && x.IsDelete == false)
        //            .OrderByDescending(x => x.UpdatedAt).ToList();
        //    var result = _mapper.Map<List<CouponGetDto>>(coupons);
        //    return Ok(new ApiResponse { Success = true, Data = result });
        //}

        //// GET: api/Coupons/GetById/5
        //[HttpGet("GetById/{id}")]
        //public async Task<IActionResult> GetCoupon(int id)
        //{
        //    var coupon = await _coupon.FindById(id);
        //    if (coupon == null || coupon.IsActive == false || coupon.IsDelete == true)
        //        return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });

        //    var result = _mapper.Map<CouponGetDto>(coupon);
        //    return Ok(new ApiResponse { Success = true, Data = result });
        //}

        // GET: api/Coupons/GetByCode/5
        [HttpGet("GetById/{code}")]
        public async Task<IActionResult> GetCoupon(string code)
        {
            var coupon = await _coupon.FindByCode(code);
            if (coupon == null || coupon.IsActive == false || coupon.IsDelete == true)
                return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });

            var result = _mapper.Map<CouponGetDto>(coupon);
            return Ok(new ApiResponse { Success = true, Data = result });
        }
    }
}
