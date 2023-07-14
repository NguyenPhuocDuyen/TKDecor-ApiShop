using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        public async Task Add(Notification entity)
            => await NotificationDAO.Add(entity);

        public async Task<Notification?> FindById(Guid id)
            => await NotificationDAO.FindById(id);

        public async Task<List<Notification>> FindByUserId(Guid userId)
            => await NotificationDAO.FindByUserId(userId);

        public async Task<List<Notification>> GetAll()
            => await NotificationDAO.GetAll();

        public async Task Update(Notification notification)
            => await NotificationDAO.Update(notification);
    }
}
