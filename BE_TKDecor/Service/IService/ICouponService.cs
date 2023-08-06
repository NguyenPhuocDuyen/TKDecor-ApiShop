using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface ICouponService
    {
        Task<ApiResponse> GetAll();
        Task<ApiResponse> Create(CouponCreateDto dto);
        Task<ApiResponse> Update(Guid id, CouponUpdateDto dto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> GetByCode(string code);
    }
}
