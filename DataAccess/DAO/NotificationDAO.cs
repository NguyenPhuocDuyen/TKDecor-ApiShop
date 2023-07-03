using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    internal class NotificationDAO
    {
        internal static async Task<List<Notification>> GetAll(long userId)
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

        internal static async Task Update(Notification notification)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(notification);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
