using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _user;

        public UsersController(IUserService user)
        {
            _user = user;
        }

        // GET: api/Users/GetUserInfo
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _user.GetById(userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Users/UpdateUserInfo
        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(UserUpdateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _user.UpdateUserInfo(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Users/RequestChangePassword
        [HttpPost("RequestChangePassword")]
        public async Task<IActionResult> RequestChangePassword()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _user.RequestChangePassword(userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Users/ChangePassword
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserChangePasswordDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _user.ChangePassword(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
