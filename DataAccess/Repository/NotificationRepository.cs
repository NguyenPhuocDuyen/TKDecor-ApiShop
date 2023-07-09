using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        public async Task<List<Notification>> GetAll(Guid userId)
            => await NotificationDAO.GetAll(userId);

        public async Task Update(Notification notification)
            => await NotificationDAO.Update(notification);
    }
}
