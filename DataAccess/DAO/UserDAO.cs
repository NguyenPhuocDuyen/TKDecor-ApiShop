using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class UserDAO
    {
        internal static async Task<List<User>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var users = await context.Users.Include(u => u.Role).ToListAsync();
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
                var user = await context.Users.Include(u => u.Role)
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
                var user = await context.Users.Include(u => u.Role)
                    .SingleOrDefaultAsync(user => user.Email == email);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static async Task Add(User user)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static async Task Update(User user)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
