using BusinessObject;
using System;
namespace DataAccess.Repository.IRepository
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<List<Notification>> FindByUserId(Guid userId);
    }
}
