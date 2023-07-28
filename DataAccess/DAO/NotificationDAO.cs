using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class NotificationDAO : DAO<Notification>
    {
        internal static async Task<List<Notification>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var notification = await context.Notifications
                    .ToListAsync();
                return notification;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<List<Notification>> FindByUserId(Guid userId)
        {
            try
            {
                using var context = new TkdecorContext();
                var notification = await context.Notifications
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
                return notification;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Notification?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                var coupon = await context.Notifications
                    .FirstOrDefaultAsync(x => x.NotificationId == id);
                return coupon;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
