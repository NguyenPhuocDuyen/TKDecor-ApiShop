using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Dtos.UserAddress;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAddressesController : ControllerBase
    {
        private readonly IUserAddressService _userAddress;

        public UserAddressesController(IUserAddressService userAddress)
        {
            _userAddress = userAddress;
        }

        // GET: api/UserAddresses/GetUserAddresses
        [HttpGet("GetUserAddresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _userAddress.GetUserAddressesForUser(userId);
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
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _userAddress.GetUserAddressDefault(userId);
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
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _userAddress.SetDefault(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/UserAddresses/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(UserAddressCreateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _userAddress.Create(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/UserAddresses/Update/1
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, UserAddressUpdateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _userAddress.Update(userId, id, dto);
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
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _userAddress.Delete(userId, id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
