using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notification;
        private readonly IUserService _user;

        public NotificationsController(INotificationService notification,
            IUserService user)
        {
            _notification = notification;
            _user = user;
        }

        // GET: api/Notifications/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetNotifications()
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _notification.GetNotificationsForUser(user.UserId);
            return Ok(res);
        }

        // POST: api/Notifications/Read
        [HttpGet("ReadAll")]
        public async Task<IActionResult> ReadAll()
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _notification.ReadAll(user.UserId);
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
