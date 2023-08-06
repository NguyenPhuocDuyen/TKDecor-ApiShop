using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementCouponsController : ControllerBase
    {
        private readonly ICouponService _coupon;

        public ManagementCouponsController(ICouponService coupon)
        {
            _coupon = coupon;
        }

        // GET: api/ManagementCoupons/GetALl
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _coupon.GetAll();
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/ManagementCoupons/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CouponCreateDto couponDto)
        {
            var res = await _coupon.Create(couponDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // PUT: api/ManagementCoupons/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, CouponUpdateDto couponDto)
        {
            var res = await _coupon.Update(id, couponDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // DELETE: api/ManagementCoupons/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _coupon.Delete(id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
