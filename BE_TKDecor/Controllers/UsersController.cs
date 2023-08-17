using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using AutoMapper;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly IMapper _mapper;

        public UsersController(IUserService user, IMapper mapper)
        {
            _user = user;
            _mapper = mapper;
        }

        // GET: api/Users/GetUserInfo
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            // get user by user id
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var result = _mapper.Map<UserGetDto>(user);

            return Ok(new ApiResponse
            { Success = true, Data = result });
        }

        // GET: api/Users/UpdateUserInfo
        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(UserUpdateDto userDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            user.FullName = userDto.FullName;
            user.AvatarUrl = userDto.AvatarUrl;
            user.BirthDay = userDto.BirthDay;
            user.Gender = userDto.Gender;
            user.Phone = userDto.Phone;
            user.UpdatedAt = DateTime.Now;

            var res = await _user.UpdateUserInfo(user);
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
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _user.RequestChangePassword(user);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Users/ChangePassword
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserChangePasswordDto userDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _user.ChangePassword(user, userDto);
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
