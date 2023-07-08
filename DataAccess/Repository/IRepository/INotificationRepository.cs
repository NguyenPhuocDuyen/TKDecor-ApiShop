using BusinessObject;
using System;
namespace DataAccess.Repository.IRepository
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAll(Guid userId);
        Task Update(Notification notification);
    }
}
