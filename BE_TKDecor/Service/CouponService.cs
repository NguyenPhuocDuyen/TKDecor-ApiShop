using AutoMapper;
using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Utility;

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

        // create coupon
        public async Task<ApiResponse> Create(CouponCreateDto dto)
        {
            try
            {
                dto.Code = dto.Code.ToUpper().Trim();

                var couponCode = await _context.Coupons.FirstOrDefaultAsync(x => x.Code == dto.Code);

                bool isAdd = true;
                if (couponCode is null)
                {
                    couponCode = _mapper.Map<Coupon>(dto);
                }
                else
                {
                    if (!couponCode.IsDelete)
                    {
                        _response.Message = "Mã giảm giá đã tồn tại!";
                        return _response;
                    }

                    isAdd = false;
                    couponCode.IsDelete = false;

                    couponCode.CouponType = dto.CouponType;
                    couponCode.Value = dto.Value;
                    couponCode.MaxValue = dto.MaxValue;
                    couponCode.RemainingUsageCount = dto.RemainingUsageCount;
                    couponCode.StartDate = dto.StartDate;
                    couponCode.EndDate = dto.EndDate;
                    couponCode.IsActive = dto.IsActive;
                    couponCode.UpdatedAt = DateTime.Now;
                }

                if (couponCode.CouponType == SD.CouponByPercent && couponCode.Value > 100)
                {
                    _response.Message = "Không thể giảm hơn 100%";
                    return _response;
                }

                if (couponCode.Value > couponCode.MaxValue)
                {
                    _response.Message = "Giá trị không được vượt quá giá trị tối đa";
                    return _response;
                }

                if (couponCode.StartDate > couponCode.EndDate)
                {
                    _response.Message = "Ngày bắt đầu và kết thúc không hợp lệ";
                    return _response;
                }

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
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // delete coupon by id
        public async Task<ApiResponse> Delete(Guid id)
        {
            var couponDb = await _context.Coupons.FindAsync(id);
            if (couponDb is null || couponDb.IsDelete)
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
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get all coupon
        public async Task<ApiResponse> GetAll()
        {
            var coupons = await _context.Coupons
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            try
            {
                var result = _mapper.Map<List<CouponGetDto>>(coupons);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get coupon by code
        public async Task<ApiResponse> GetByCode(string code)
        {
            code = code.ToUpper().Trim();

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(x => x.Code == code && !x.IsDelete);
            if (coupon is null)
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

            try
            {
                var result = _mapper.Map<CouponGetDto>(coupon);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // update coupon
        public async Task<ApiResponse> Update(Guid id, CouponUpdateDto dto)
        {
            if (id != dto.CouponId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var couponDb = await _context.Coupons.FindAsync(id);
            if (couponDb is null || couponDb.IsDelete)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            couponDb.CouponType = dto.CouponType;
            couponDb.Value = dto.Value;
            couponDb.MaxValue = dto.MaxValue;
            couponDb.RemainingUsageCount = dto.RemainingUsageCount;
            couponDb.StartDate = dto.StartDate;
            couponDb.EndDate = dto.EndDate;
            couponDb.IsActive = dto.IsActive;
            couponDb.UpdatedAt = DateTime.Now;
            try
            {
                _context.Coupons.Update(couponDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
