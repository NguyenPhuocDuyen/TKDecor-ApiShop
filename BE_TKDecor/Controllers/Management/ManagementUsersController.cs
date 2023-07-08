using AutoMapper;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Admin)]
    public class ManagementUsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly IRoleRepository _role;

        public ManagementUsersController(IMapper mapper,
            IUserRepository user,
            IRoleRepository role)
        {
            _mapper = mapper;
            _user = user;
            _role = role;
        }

        // GET: api/ManagementUsers/GetAllUser
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _user.GetAll();
            var result = _mapper.Map<List<UserGetDto>>(user);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/ManagementUsers/SetRole
        [HttpPut("SetRole/{userId}")]
        public async Task<IActionResult> GetUserInfo(Guid userId, UserSetRoleDto userDto)
        {
            if (userId != userDto.UserId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var user = await _user.FindById(userId);
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var role = await _role.FindByName(userDto.RoleName);
            if (role == null)
                return NotFound(new ApiResponse { Message = ErrorContent.RoleNotFound });

            user.RoleId = role.RoleId;
            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _user.Update(user);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
