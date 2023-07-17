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

        // GET: api/Coupons/GetByCode/5
        [HttpGet("GetByCode/{code}")]
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
