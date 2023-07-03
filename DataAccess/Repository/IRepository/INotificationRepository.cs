using BusinessObject;
using System;
namespace DataAccess.Repository.IRepository
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAll(long userId);
        Task Update(Notification notification);
    }
}
