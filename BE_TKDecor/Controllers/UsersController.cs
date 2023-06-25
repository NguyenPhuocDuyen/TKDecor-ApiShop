using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using AutoMapper;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UsersController(IMapper mapper,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // GET: api/Users/GetUserInfo
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            // get user by user id
            var user = await GetUser();
            var result = _mapper.Map<UserGetDto>(user);

            return Ok(new ApiResponse
            { Success = true, Data = result });
        }

        // GET: api/Users/UpdateUserInfo
        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(UserUpdateDto userDto)
        {
            var user = await GetUser();
            if (user == null)
                return BadRequest(new ApiResponse
                { Message = ErrorContent.UserNotFound });

            user.FullName = userDto.FullName;
            user.AvatarUrl = userDto.AvatarUrl;
            user.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _userRepository.Update(user);
                return NoContent();
            }
            catch
            {
                return BadRequest(new ApiResponse { Message = ErrorContent.Data });
            }
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _userRepository.FindById(int.Parse(userId));
            }
            return null;
        }
    }
}
