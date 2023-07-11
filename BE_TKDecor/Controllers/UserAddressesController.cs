using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Dtos.UserAddress;
using BE_TKDecor.Core.Response;
using DataAccess.StatusContent;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAddressesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly IUserAddressRepository _userAddress;

        public UserAddressesController(IMapper mapper,
            IUserRepository user,
            IUserAddressRepository userAddress)
        {
            _mapper = mapper;
            _user = user;
            _userAddress = userAddress;
        }

        // GET: api/UserAddresses/GetUserAddresses
        [HttpGet("GetUserAddresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var list = (await _userAddress.FindByUserId(user.UserId))
                .Where(x => x.IsDelete == false)
                .OrderByDescending(x => x.UpdatedAt);
            var result = _mapper.Map<List<UserAddressGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/UserAddresses/SetDefault
        [HttpPost("SetDefault")]
        public async Task<IActionResult> SetDefault(UserAddressSetDefaultDto dto)
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var address = await _userAddress.FindById(dto.UserAddressId);
            if (address == null || address.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.AddressNotFound });

            try
            {
                await _userAddress.SetDefault(user.UserId, dto.UserAddressId);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/UserAddresses/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(UserAddressCreateDto userAddressDto)
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            UserAddress newAddress = _mapper.Map<UserAddress>(userAddressDto);
            newAddress.UserId = user.UserId;
            try
            {
                await _userAddress.Add(newAddress);

                var listAddress = await _userAddress.FindByUserId(user.UserId);
                if (listAddress.Count <= 1)
                {
                    await _userAddress.SetDefault(user.UserId, null);
                }

                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/UserAddresses/Update/1
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUserAddress(Guid id, UserAddressUpdateDto userAddressDto)
        {
            if (id != userAddressDto.UserAddressId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var userAddressDb = await _userAddress.FindById(id);
            if (userAddressDb == null || userAddressDb.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.AddressNotFound });

            userAddressDb.FullName = userAddressDto.FullName;
            userAddressDb.Address = userAddressDto.Address;
            userAddressDb.Phone = userAddressDto.Phone;
            userAddressDb.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _userAddress.Update(userAddressDb);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/UserAddresses/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUserAddress(Guid id)
        {
            var userAddress = await _userAddress.FindById(id);
            if (userAddress == null)
                return NotFound(new ApiResponse { Message = ErrorContent.AddressNotFound });

            if (userAddress.IsDefault)
                return BadRequest(new ApiResponse { Message = "Can't not delete default address!" });

            userAddress.IsDelete = true;
            userAddress.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _userAddress.Update(userAddress);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.FindById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
