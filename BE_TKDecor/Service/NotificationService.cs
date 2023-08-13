using AutoMapper;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Service
{
    public class NotificationService : INotificationService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public NotificationService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetNotificationsForUser(Guid userId)
        {
            var notifications = await _context.Notifications
                    .Where(x => x.UserId == userId && !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            try
            {
                var result = _mapper.Map<List<NotificationGetDto>>(notifications);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> ReadAll(Guid userId)
        {
            var notifications = await _context.Notifications.Where(x => x.UserId == userId && !x.IsRead).ToListAsync();

            foreach (var item in notifications)
            {
                item.IsRead = true;
                item.UpdatedAt = DateTime.Now;
            }
            
            try
            {
                _context.Notifications.UpdateRange(notifications);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
