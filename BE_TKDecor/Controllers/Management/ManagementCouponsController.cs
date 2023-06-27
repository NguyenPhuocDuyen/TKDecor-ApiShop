using AutoMapper;
using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Seller},{RoleContent.Admin}")]
    public class ManagementCouponsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICouponRepository _couponRepository;

        public ManagementCouponsController(IMapper mapper,
            ICouponRepository couponRepository)
        {
            _mapper = mapper;
            _couponRepository = couponRepository;
        }

        // GET: api/ManagementCoupons/GetALl
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var coupons = (await _couponRepository.GetAll())
                   .OrderByDescending(x => x.UpdatedAt).ToList();
            var result = _mapper.Map<List<CouponGetDto>>(coupons);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/ManagementCoupons/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CouponCreateDto couponDto)
        {
            var couponCode = await _couponRepository.FindByCode(couponDto.Code);
            if (couponCode != null)
                return BadRequest(new ApiResponse { Message = "Coupon code already exists!" });

            Coupon newCoupon = _mapper.Map<Coupon>(couponCode);
            try
            {
                await _couponRepository.Add(newCoupon);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // PUT: api/ManagementCoupons/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> PutCoupon(int id, CouponUpdateDto couponDto)
        {
            if (id != couponDto.CouponId)
                return BadRequest(new ApiResponse { Message = "ID does not match!" });

            var couponDb = await _couponRepository.FindById(id);
            if (couponDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });

            couponDb.CouponTypeId = couponDto.CouponTypeId;
            couponDb.Value = couponDto.Value;
            couponDb.RemainingUsageCount = couponDto.RemainingUsageCount;
            couponDb.StartDate = couponDto.StartDate;
            couponDb.EndDate = couponDto.EndDate;
            couponDb.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _couponRepository.Update(couponDb);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/ManagementCoupons/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var couponDb = await _couponRepository.FindById(id);
            if (couponDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });

            couponDb.UpdatedAt = DateTime.UtcNow;
            couponDb.IsActive = false;
            try
            {
                await _couponRepository.Update(couponDb);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
