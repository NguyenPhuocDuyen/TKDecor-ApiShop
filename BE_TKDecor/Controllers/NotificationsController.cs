using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Response;
using DataAccess.Repository.IRepository;
using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly INotificationRepository _notification;

        public NotificationsController(IMapper mapper,
            IUserRepository user,
            INotificationRepository notification)
        {
            _mapper = mapper;
            _user = user;
            _notification = notification;
        }

        // GET: api/Notifications/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetNotifications()
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var notifications = await _notification.FindByUserId(user.UserId);
            notifications = notifications.Where(x => x.IsDelete == false).ToList();
            var result = _mapper.Map<List<NotificationGetDto>>(notifications);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/Notifications/GetAll
        [HttpPost("ReadAll")]
        public async Task<IActionResult> ReadAll()
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var notifications = await _notification.FindByUserId(user.UserId);
            notifications = notifications.Where(x => x.IsRead == false).ToList();
            foreach (var item in notifications)
            {
                item.IsRead = true;
                await _notification.Update(item);
            }
            return NoContent();
        }

        // GET: api/Notifications/Subscribe
        [HttpGet("Subscribe/{isSubscribe}")]
        public async Task<IActionResult> Subscribe(bool isSubscribe = true)
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            user.IsSubscriber = isSubscribe;
            user.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _user.Update(user);
                return NoContent();
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
