using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using AutoMapper;
using Utility;
using BE_TKDecor.Hubs;
using Microsoft.AspNetCore.SignalR;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Mail;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly ISendMailService _sendMailService;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;

        public UsersController(IMapper mapper,
            IUserRepository user,
            ISendMailService sendMailService,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub
            )
        {
            _mapper = mapper;
            _user = user;
            _sendMailService = sendMailService;
            _notification = notification;
            _hub = hub;
        }

        // GET: api/Users/GetUserInfo
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            // get user by user id
            var user = await GetUser();
            if (user == null || user.IsDelete)
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
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            user.FullName = userDto.FullName;
            user.AvatarUrl = userDto.AvatarUrl;
            user.BirthDay = userDto.BirthDay;
            user.Gender = userDto.Gender;
            user.Phone = userDto.Phone;
            user.UpdatedAt = DateTime.Now;
            try
            {
                await _user.Update(user);

                //// add notification for user
                //Notification newNotification = new()
                //{
                //    UserId = user.UserId,
                //    Message = $"Cập nhật thông tin thành công"
                //};
                //await _notification.Add(newNotification);
                //// notification signalR
                //await _hub.Clients.User(user.UserId.ToString())
                //    .SendAsync(Common.NewNotification,
                //    _mapper.Map<NotificationGetDto>(newNotification));

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // GET: api/Users/RequestChangePassword
        [HttpPost("RequestChangePassword")]
        public async Task<IActionResult> RequestChangePassword()
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            // get random code
            string code = RandomCode.GenerateRandomCode();
            user.ResetPasswordRequired = true;
            user.ResetPasswordCode = code;
            user.ResetPasswordSentAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            try
            {
                // update user
                await _user.Update(user);

                //send mail to changepassword
                //set data to send
                MailContent mailContent = new()
                {
                    To = user.Email,
                    Subject = "Đổi mật khẩu tại TKDecor Shop",
                    Body = $"<h4>Bạn yêu cầu đổi mật khẩu cho web TKDecor. " +
                    $"Nếu bạn không có yêu cầu đổi mật khẩu, hãy bỏ qua email này!</h4>" +
                    $"<p>Đây là mã xác nhận đổi mật khẩu: <strong>{code}</strong></p>"
                };
                // send mail
                await _sendMailService.SendMail(mailContent);

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Bạn đã yêu cầu thay đổi mật khẩu"
                };
                await _notification.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // GET: api/Users/ChangePassword
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserChangePasswordDto userDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            if (!user.ResetPasswordRequired)
                return BadRequest(new ApiResponse { Message = "Người dùng không yêu cầu đổi mật khẩu!" });

            if (user.ResetPasswordCode != userDto.Code)
                return BadRequest(new ApiResponse { Message = "Sai mã xác nhận!" });

            //check correct password
            bool isCorrectPassword = Password.VerifyPassword(userDto.Password, user.Password);
            if (!isCorrectPassword)
                return BadRequest(new ApiResponse { Message = "Sai mật khẩu!" });

            bool codeOutOfDate = false;
            if (user.ResetPasswordSentAt > DateTime.Now.AddMinutes(-5))
            {
                codeOutOfDate = true;
            }

            // random new code if code time expired
            if (codeOutOfDate)
            {
                // get random code
                string code = RandomCode.GenerateRandomCode();
                user.ResetPasswordCode = code;
            }
            else
            {
                user.Password = Password.HashPassword(userDto.NewPassword);
                user.ResetPasswordRequired = false;
            }
            user.UpdatedAt = DateTime.Now;
            try
            {
                // update success before send new code for user
                await _user.Update(user);

                if (codeOutOfDate)
                {
                    //send mail to changepassword
                    //set data to send
                    MailContent mailContent = new()
                    {
                        To = user.Email,
                        Subject = "Đổi mật khẩu tại TKDecor Shop",
                        Body = $"<h4>Bạn yêu cầu đổi mật khẩu cho web TKDecor. " +
                        $"Nếu bạn không có yêu cầu đổi mật khẩu, hãy bỏ qua email này!</h4>" +
                        $"<p>Đây là mã xác nhận: <strong>{user.ResetPasswordCode}</strong></p>"
                    };
                    // send mail
                    await _sendMailService.SendMail(mailContent);
                    return BadRequest(new ApiResponse { Message = "Hết thời gian mã xác nhận đổi mật khẩu. Vui lòng kiểm tra lại mail để xem mã mới!" });
                }

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Đổi mật khẩu thành công"
                };
                await _notification.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

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
