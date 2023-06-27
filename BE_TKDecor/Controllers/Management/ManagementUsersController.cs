using AutoMapper;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Admin)]
    public class ManagementUsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public ManagementUsersController(IMapper mapper,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        // GET: api/ManagementUsers/GetAllUser
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _userRepository.GetAll();
            var result = _mapper.Map<List<UserGetDto>>(user);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/ManagementUsers/SetRole
        [HttpPut("SetRole/{userId}")]
        public async Task<IActionResult> GetUserInfo(int userId, UserSetRoleDto userDto)
        {
            if (userId != userDto.UserId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var user = await _userRepository.FindById(userId);
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var role = await _roleRepository.FindByName(userDto.RoleName);
            if (role == null)
                return NotFound(new ApiResponse { Message = ErrorContent.RoleNotFound });

            user.RoleId = role.RoleId;
            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _userRepository.Update(user);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
