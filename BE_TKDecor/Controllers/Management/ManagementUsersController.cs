using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementUsersController : ControllerBase
    {
        private readonly IUserService _user;

        public ManagementUsersController(IUserService user)
        {
            _user = user;
        }

        // GET: api/ManagementUsers/GetAllUser
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _user.GetAllUser(userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // PUT: api/ManagementUsers/SetRole/1
        [HttpPut("SetRole/{userId}")]
        public async Task<IActionResult> SetRole(Guid userId, UserSetRoleDto dto)
        {
            var res = await _user.SetRole(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
