using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class UserDAO : DAO<User>
    {
        internal static async Task<List<User>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var users = await context.Users
                    .ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static async Task<User?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                var user = await context.Users
                    .SingleOrDefaultAsync(user => user.UserId == id);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static async Task<User?> FindByEmail(string email)
        {
            try
            {
                using var context = new TkdecorContext();
                var user = await context.Users
                    .SingleOrDefaultAsync(user => user.Email == email);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
