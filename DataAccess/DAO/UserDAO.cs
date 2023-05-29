using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace DataAccess.DAO
{
    public class UserDAO
    {
        public static async Task<List<User>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var users = await context.Users.Include(u => u.Role).ToListAsync();
                users.ForEach(u => { u.Password = ""; });
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<bool> CheckLogin(string email, string password)
        {
            try
            {
                bool isUserExists = false;

                using var context = new TkdecorContext();
                var user = await context.Users
                    .SingleOrDefaultAsync(user => user.Email == email);

                if (user is not null)
                    if (Password.VerifyPassword(password, user.Password)) return true;

                return isUserExists;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<User> FindById(int id)
        {
            try
            {
                using var context = new TkdecorContext();
                var user = await context.Users.Include(u => u.Role).SingleOrDefaultAsync(user => user.UserId == id);

                if (user != null) user.Password = "";

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<User> FindByEmail(string email)
        {
            try
            {
                using var context = new TkdecorContext();
                var user = await context.Users.Include(u => u.Role).SingleOrDefaultAsync(user => user.Email == email);

                if (user != null) user.Password = "";

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task Add(User user)
        {
            try
            {
                using var context = new TkdecorContext();
                //hash password
                user.Password = Password.HashPassword(user.Password);
                await context.AddAsync(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task Update(User user)
        {
            try
            {
                using var context = new TkdecorContext();
                // hash password
                user.Password = Password.HashPassword(user.Password);
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
