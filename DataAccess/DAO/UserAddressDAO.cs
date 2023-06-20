using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class UserAddressDAO
    {
        public static async Task<List<UserAddress>> GetByUserId(int userId)
        {
            try
            {
                using var context = new TkdecorContext();
                var list = await context.UserAddresses
                    .Where(x => x.UserId == userId).ToListAsync();
                return list;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task<UserAddress?> FindById(int id)
        {
            try
            {
                using var context = new TkdecorContext();
                var userAddress = await context.UserAddresses
                    .FirstOrDefaultAsync(x => x.UserAddressId == id);
                return userAddress;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task Add(UserAddress userAddress)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(userAddress);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task Update(UserAddress userAddress)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(userAddress);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task Delete(UserAddress userAddress)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Remove(userAddress);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static async Task SetDefault(int userId, int? userAddressId)
        {
            try
            {
                using var context = new TkdecorContext();
                var listAddressOfUser = context.UserAddresses.Where(x => x.UserId == userId);
                var addressDefault = listAddressOfUser.FirstOrDefault();
                foreach (var address in listAddressOfUser)
                {
                    address.IsDefault = false;
                    if (userAddressId != null)
                    {
                        if (userAddressId == address.UserAddressId)
                        {
                            addressDefault = address;
                        }
                    }
                }
                if (addressDefault != null)
                    addressDefault.IsDefault = true;
                
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
