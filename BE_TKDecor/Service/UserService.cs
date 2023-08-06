using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.User;
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
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hub;
        private ApiResponse _response;

        public UserService(TkdecorContext context,
            IMapper mapper,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _hub = hub;
            _response = new ApiResponse();
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
    }
}
