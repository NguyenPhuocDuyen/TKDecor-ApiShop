using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Dtos.UserAddress;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAddressesController : ControllerBase
    {
        private readonly IUserAddressService _userAddress;
        private readonly IUserService _user;

        public UserAddressesController(IUserAddressService userAddress,
            IUserService user)
        {
            _userAddress = userAddress;
            _user = user;
        }

        // GET: api/UserAddresses/GetUserAddresses
        [HttpGet("GetUserAddresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _userAddress.GetUserAddressesForUser(user.UserId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/UserAddresses/GetUserAddressDefault
        [HttpGet("GetUserAddressDefault")]
        public async Task<IActionResult> GetUserAddressDefault()
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _userAddress.GetUserAddressDefault(user.UserId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/UserAddresses/SetDefault
        [HttpPost("SetDefault")]
        public async Task<IActionResult> SetDefault(UserAddressSetDefaultDto dto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _userAddress.SetDefault(user.UserId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/UserAddresses/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(UserAddressCreateDto userAddressDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _userAddress.Create(user.UserId, userAddressDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/UserAddresses/Update/1
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, UserAddressUpdateDto userAddressDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _userAddress.Update(user.UserId, id, userAddressDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // DELETE: api/UserAddresses/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _userAddress.Delete(user.UserId, id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.GetById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
