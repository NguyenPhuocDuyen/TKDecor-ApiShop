using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var users = await context.Users.ToListAsync();
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
                    if (Password.VerifyPassword(password, user.PasswordHash)) return true;

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
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<User> Add(User user)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                var newUser = await context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
                return newUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<User> Update(User user)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Users.Update(user);
                await context.SaveChangesAsync();

                var newUser = await context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
                return newUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
