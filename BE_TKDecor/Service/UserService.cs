using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Mail;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class UserService : IUserService
    {
        private readonly TkdecorContext _context;
        private readonly ISendMailService _sendMailService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hub;
        private ApiResponse _response;

        public UserService(TkdecorContext context,
            ISendMailService sendMailService,
            IMapper mapper,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _sendMailService = sendMailService;
            _mapper = mapper;
            _hub = hub;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> ChangePassword(User user, UserChangePasswordDto dto)
        {
            if (!user.ResetPasswordRequired)
            {
                _response.Message = "Người dùng không yêu cầu đổi mật khẩu!";
                return _response;
            }

            if (user.ResetPasswordCode != dto.Code)
            {
                _response.Message = "Sai mã xác nhận!";
                return _response;
            }

            //check correct password
            bool isCorrectPassword = Password.VerifyPassword(dto.Password, user.Password);
            if (!isCorrectPassword)
            {
                _response.Message = "Sai mật khẩu!";
                return _response;
            }

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
                user.Password = Password.HashPassword(dto.NewPassword);
                user.ResetPasswordRequired = false;
            }
            user.UpdatedAt = DateTime.Now;
            try
            {
                // update success before send new code for user
                _context.Users.Update(user);

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Đổi mật khẩu thành công"
                };
                _context.Notifications.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                await _context.SaveChangesAsync();
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
                    _response.Message = "Hết thời gian mã xác nhận đổi mật khẩu. Vui lòng kiểm tra lại mail để xem mã mới!";
                    return _response;
                }
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> Delete(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDelete)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            user.IsDelete = true;
            user.UpdatedAt = DateTime.Now;
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> GetAllUser(Guid userId)
        {
            var users = await _context.Users.ToListAsync();
            users = users.Where(x => !x.IsDelete && x.UserId != userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<UserGetDto>>(users);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<User?> GetById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<ApiResponse> RequestChangePassword(User user)
        {
            // get random code
            string code = RandomCode.GenerateRandomCode();
            user.ResetPasswordRequired = true;
            user.ResetPasswordCode = code;
            user.ResetPasswordSentAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            try
            {
                // update user
                _context.Users.Update(user);

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
                _context.Notifications.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> SetRole(Guid userId, UserSetRoleDto dto)
        {
            if (userId != dto.UserId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDelete)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            user.Role = dto.Role;
            user.UpdatedAt = DateTime.Now;
            try
            {
                var refreshTokenOfUser = await _context.RefreshTokens.Where(x => x.UserId == userId).ToListAsync();
                _context.RefreshTokens.RemoveRange(refreshTokenOfUser);

                _context.Users.Update(user);

                // add notification for user
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = $"Vai trò của bạn được quản trị thay đổi thành {dto.Role}"
                };
                _context.Notifications.Add(newNotification);
                // notification signalR
                await _hub.Clients.User(user.UserId.ToString())
                    .SendAsync(SD.NewNotification,
                    _mapper.Map<NotificationGetDto>(newNotification));

                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> UpdateUserInfo(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
