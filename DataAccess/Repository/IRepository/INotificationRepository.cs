using BusinessObject;
using System;
namespace DataAccess.Repository.IRepository
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAll(int userId);
        Task Update(Notification notification);
    }
}
