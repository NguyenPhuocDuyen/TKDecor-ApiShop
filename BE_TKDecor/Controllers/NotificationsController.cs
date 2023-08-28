using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notification;

        public NotificationsController(INotificationService notification)
        {
            _notification = notification;
        }

        // GET: api/Notifications/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _notification.GetNotificationsForUser(userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Notifications/ReadAll
        [HttpGet("ReadAll")]
        public async Task<IActionResult> ReadAll()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _notification.ReadAll(userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
