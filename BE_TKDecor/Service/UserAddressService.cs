using AutoMapper;
using BE_TKDecor.Core.Dtos.UserAddress;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Service
{
    public class UserAddressService : IUserAddressService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private readonly ApiResponse _response;

        public UserAddressService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        // create address
        public async Task<ApiResponse> Create(string? userId, UserAddressCreateDto dto)
        {
            if (userId is null)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            try
            {
                UserAddress newAddress = _mapper.Map<UserAddress>(dto);
                newAddress.UserId = Guid.Parse(userId);
                _context.UserAddresses.Add(newAddress);
                await _context.SaveChangesAsync();

                // set as default address when there is a first address
                var listAddress = await _context.UserAddresses
                    .Where(x => x.UserId.ToString() == userId && !x.IsDelete)
                    .ToListAsync();
                if (listAddress.Count == 1)
                {
                    listAddress[0].IsDefault = true;
                    _context.UserAddresses.Update(listAddress[0]);
                }
                await _context.SaveChangesAsync();

                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // delete address by id
        public async Task<ApiResponse> Delete(string? userId, Guid userAddressId)
        {
            var userAddress = await _context.UserAddresses
                .FirstOrDefaultAsync(x => x.UserAddressId == userAddressId
                                    && x.UserId.ToString() == userId
                                    && !x.IsDelete);

            if (userAddress is null)
            {
                _response.Message = ErrorContent.AddressNotFound;
                return _response;
            }

            if (userAddress.IsDefault)
            {
                _response.Message = "Không thể xóa địa chỉ mặc định!";
                return _response;
            }

            userAddress.IsDelete = true;
            userAddress.UpdatedAt = DateTime.Now;
            try
            {
                _context.UserAddresses.Update(userAddress);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get address default for user
        public async Task<ApiResponse> GetUserAddressDefault(string? userId)
        {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(x => !x.IsDelete
                                    && x.IsDefault
                                    && x.UserId.ToString() == userId);

            if (address is null)
            {
                _response.Message = ErrorContent.AddressNotFound;
                return _response;
            }

            try
            {
                var result = _mapper.Map<UserAddressGetDto>(address);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get list of address for user
        public async Task<ApiResponse> GetUserAddressesForUser(string? userId)
        {
            var list = await _context.UserAddresses
                .Where(x => !x.IsDelete && x.UserId.ToString() == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            try
            {
                var result = _mapper.Map<List<UserAddressGetDto>>(list);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // set address default
        public async Task<ApiResponse> SetDefault(string? userId, UserAddressSetDefaultDto dto)
        {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(x => x.UserAddressId == dto.UserAddressId
                                    && !x.IsDelete
                                    && x.UserId.ToString() == userId);

            if (address is null)
            {
                _response.Message = ErrorContent.AddressNotFound;
                return _response;
            }

            // get address default
            var addressDefaults = await _context.UserAddresses
                .Where(x => x.UserId.ToString() == userId && !x.IsDelete && x.IsDefault)
                .ToListAsync();

            try
            {
                // set as default address and un - default in other addresses
                address.IsDefault = true;
                address.UpdatedAt = DateTime.Now;
                _context.UserAddresses.Update(address);

                foreach (var ad in addressDefaults)
                {
                    ad.IsDefault = false;
                    ad.UpdatedAt = DateTime.Now;
                    _context.UserAddresses.Update(ad);
                }

                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // update address
        public async Task<ApiResponse> Update(string? userId, Guid userAddressId, UserAddressUpdateDto dto)
        {
            if (userAddressId != dto.UserAddressId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var userAddressDb = await _context.UserAddresses
                .FirstOrDefaultAsync(x => x.UserAddressId == userAddressId
                                    && !x.IsDelete
                                    && x.UserId.ToString() == userId);

            if (userAddressDb is null)
            {
                _response.Message = ErrorContent.AddressNotFound;
                return _response;
            }

            userAddressDb.FullName = dto.FullName;
            userAddressDb.Phone = dto.Phone;
            userAddressDb.CityCode = dto.CityCode;
            userAddressDb.City = dto.City;
            userAddressDb.DistrictCode = dto.DistrictCode;
            userAddressDb.District = dto.District;
            userAddressDb.WardCode = dto.WardCode;
            userAddressDb.Ward = dto.Ward;
            userAddressDb.Street = dto.Street;
            userAddressDb.UpdatedAt = DateTime.Now;
            try
            {
                _context.UserAddresses.Update(userAddressDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
