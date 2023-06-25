using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Mvc.Formatters;
using DataAccess.DAO;
using System.Security.AccessControl;
using BE_TKDecor.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using DataAccess.StatusContent;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Seller},{RoleContent.Admin}")]
    public class CouponsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICouponRepository _couponRepository;

        public CouponsController(IMapper mapper,
            ICouponRepository couponRepository)
        {
            _mapper = mapper;
            _couponRepository = couponRepository;
        }

        // GET: api/Coupons/GetALl
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var coupons = (await _couponRepository.GetAll())
                   .OrderByDescending(x => x.UpdatedAt).ToList();
            var result = _mapper.Map<List<CouponGetDto>>(coupons);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Coupons/GetById/5
        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCoupon(int id)
        {
            var coupon = await _couponRepository.FindById(id);
            if (coupon == null)
                return NotFound(new ApiResponse { Message = ErrorContent.CouponNotFound });
            var result = _mapper.Map<CouponGetDto>(coupon);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/Coupons/Create
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

        // PUT: api/Coupons/Update/5
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

        // DELETE: api/Coupons/Delete/5
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
