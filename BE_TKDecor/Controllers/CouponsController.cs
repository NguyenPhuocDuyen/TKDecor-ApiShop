using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _coupon;

        public CouponsController(ICouponService coupon)
        {
            _coupon = coupon;
        }

        // GET: api/Coupons/GetByCode/5
        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var res = await _coupon.GetByCode(code);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
