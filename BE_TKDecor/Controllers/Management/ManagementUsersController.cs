using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Composition;
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
        private readonly IRefreshTokenRepository _refreshToken;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;

        public ManagementUsersController(IMapper mapper,
            IUserRepository user,
            IRefreshTokenRepository refreshToken,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub)
        {
            _mapper = mapper;
            _user = user;
            _refreshToken = refreshToken;
            _notification = notification;
            _hub = hub;
        }

        // GET: api/ManagementUsers/GetAllUser
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var users = await _user.GetAll();
            users = users.Where(x => !x.IsDelete && x.UserId != user.UserId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();

            var result = _mapper.Map<List<UserGetDto>>(users);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/ManagementUsers/SetRole
        [HttpPut("SetRole/{userId}")]
        public async Task<IActionResult> GetUserInfo(Guid userId, UserSetRoleDto userDto)
        {
            if (userId != userDto.UserId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var user = await _user.FindById(userId);
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            if (!Enum.TryParse(userDto.Role, out Role role))
                return NotFound(new ApiResponse { Message = ErrorContent.RoleNotFound });

            user.Role = role;
            user.UpdatedAt = DateTime.Now;
            try
            {
                var refreshTokenOfUser = await _refreshToken.FindByUserId(userId);
                foreach (var refresh in refreshTokenOfUser)
                {
                    await _refreshToken.Delete(refresh);
                }
                await _user.Update(user);

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Vai trò của bạn được quản trị thay đổi thành {userDto.Role}"
                };
                await _notification.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(Common.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        [HttpDelete("Delete/{userId}")]
        public async Task<IActionResult> GetUserInfo(Guid userId)
        {
            var user = await _user.FindById(userId);
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            user.IsDelete = true;
            user.UpdatedAt = DateTime.Now;
            try
            {
                await _user.Update(user);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.FindById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
