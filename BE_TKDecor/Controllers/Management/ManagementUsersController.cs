using AutoMapper;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = RoleContent.Admin)]
    public class ManagementUsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;

        public ManagementUsersController(IMapper mapper,
            IUserRepository user)
        {
            _mapper = mapper;
            _user = user;
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

            Role role;
            if (!Enum.TryParse<Role>(userDto.Role, out role))
            {
                return NotFound(new ApiResponse { Message = ErrorContent.RoleNotFound });
            }

            user.Role = role;
            user.UpdatedAt = DateTime.Now;
            try
            {
                await _user.Update(user);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
