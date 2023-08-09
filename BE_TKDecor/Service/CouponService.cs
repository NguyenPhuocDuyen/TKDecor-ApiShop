using AutoMapper;
using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Service
{
    public class CouponService : ICouponService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public CouponService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> Create(CouponCreateDto dto)
        {
            bool isAdd = true;
            var couponCode = await _context.Coupons.FirstOrDefaultAsync(x => x.Code == dto.Code);
            if (couponCode == null)
            {
                couponCode = _mapper.Map<Coupon>(dto);
                couponCode.Code = dto.Code.ToLower();
            }
            else
            {
                if (!couponCode.IsDelete)
                {
                    _response.Message = "Mã giảm giá đã tồn tại!";
                    return _response;
                }

                couponCode.IsDelete = false;
                isAdd = false;

                couponCode.IsActive = dto.IsActive;
                couponCode.Value = dto.Value;
                couponCode.MaxValue = dto.MaxValue;
                couponCode.RemainingUsageCount = dto.RemainingUsageCount;
                couponCode.StartDate = dto.StartDate;
                couponCode.EndDate = dto.EndDate;
                couponCode.UpdatedAt = DateTime.Now;
            }
            couponCode.CouponType = dto.CouponType;

            try
            {
                if (isAdd)
                {
                    _context.Coupons.Add(couponCode);
                }
                else
                {
                    _context.Coupons.Update(couponCode);
                }
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var couponDb = await _context.Coupons.FindAsync(id);
            if (couponDb == null || couponDb.IsDelete)
            {
                _response.Message = ErrorContent.CouponNotFound;
                return _response;
            }

            couponDb.UpdatedAt = DateTime.Now;
            couponDb.IsDelete = true;
            try
            {
                _context.Coupons.Update(couponDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> GetAll()
        {
            var coupons = await _context.Coupons.ToListAsync();
            coupons = coupons.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<CouponGetDto>>(coupons);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> GetByCode(string code)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Code == code.ToLower());
            if (coupon == null || coupon.IsDelete)
            {
                _response.Message = ErrorContent.CouponNotFound;
                return _response;
            }

            var currentDay = DateTime.Now;
            if (currentDay < coupon.StartDate || currentDay > coupon.EndDate || !coupon.IsActive)
            {
                _response.Message = "Mã giảm giá không có hiệu lực";
                return _response;
            }

            if (coupon.RemainingUsageCount == 0)
            {
                _response.Message = "Mã giảm giá hết lượt sử dụng";
                return _response;
            }

            var result = _mapper.Map<CouponGetDto>(coupon);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> Update(Guid id, CouponUpdateDto dto)
        {
            if (id != dto.CouponId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var couponDb = await _context.Coupons.FindAsync(id);
            if (couponDb == null || couponDb.IsDelete)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            couponDb.CouponType = dto.CouponType;
            couponDb.IsActive = dto.IsActive;
            couponDb.Value = dto.Value;
            couponDb.MaxValue = dto.MaxValue;
            couponDb.RemainingUsageCount = dto.RemainingUsageCount;
            couponDb.StartDate = dto.StartDate;
            couponDb.EndDate = dto.EndDate;
            couponDb.UpdatedAt = DateTime.Now;
            try
            {
                _context.Coupons.Update(couponDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }
    }
}
