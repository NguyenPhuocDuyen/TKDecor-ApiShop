using AutoMapper;
using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Seller},{RoleContent.Admin}")]
    public class ManagementCouponsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICouponRepository _coupon;

        public ManagementCouponsController(IMapper mapper,
            ICouponRepository coupon)
        {
            _mapper = mapper;
            _coupon = coupon;
        }

        // GET: api/ManagementCoupons/GetALl
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var coupons = (await _coupon.GetAll())
                   .OrderByDescending(x => x.UpdatedAt).ToList();
            var result = _mapper.Map<List<CouponGetDto>>(coupons);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/ManagementCoupons/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CouponCreateDto couponDto)
        {
            var couponCode = await _coupon.FindByCode(couponDto.Code);
            if (couponCode != null)
                return BadRequest(new ApiResponse { Message = "Coupon code already exists!" });

            if (!Enum.TryParse(couponDto.CouponType, out CouponType couponType))
                return BadRequest(new ApiResponse { Message = ErrorContent.CouponTypeNotFound });

            couponCode = _mapper.Map<Coupon>(couponDto);
            couponCode.CouponType = couponType;
            try
            {
                await _coupon.Add(couponCode);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // PUT: api/ManagementCoupons/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> PutCoupon(Guid id, CouponUpdateDto couponDto)
        {
            if (id != couponDto.CouponId)
                return BadRequest(new ApiResponse { Message = "ID does not match!" });

            var couponDb = await _coupon.FindById(id);
            if (couponDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });

            if (!Enum.TryParse(couponDto.CouponType, out CouponType couponType))
                return BadRequest(new ApiResponse { Message = ErrorContent.CouponTypeNotFound });

            couponDb.CouponType = couponType;
            couponDb.Value = couponDto.Value;
            couponDb.MaxValue = couponDto.MaxValue;
            couponDb.RemainingUsageCount = couponDto.RemainingUsageCount;
            couponDb.StartDate = couponDto.StartDate;
            couponDb.EndDate = couponDto.EndDate;
            couponDb.UpdatedAt = DateTime.Now;
            try
            {
                await _coupon.Update(couponDb);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/ManagementCoupons/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCoupon(Guid id)
        {
            var couponDb = await _coupon.FindById(id);
            if (couponDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });

            couponDb.UpdatedAt = DateTime.Now;
            couponDb.IsActive = false;
            try
            {
                await _coupon.Update(couponDb);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
