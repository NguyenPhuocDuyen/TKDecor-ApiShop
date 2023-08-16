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
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _user.GetAllUser(user.UserId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // PUT: api/ManagementUsers/SetRole
        [HttpPut("SetRole/{userId}")]
        public async Task<IActionResult> SetRole(Guid userId, UserSetRoleDto userDto)
        {
            var res = await _user.SetRole(userId, userDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        [HttpDelete("Delete/{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var res = await _user.Delete(userId);
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
