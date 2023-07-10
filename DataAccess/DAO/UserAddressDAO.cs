using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class UserAddressDAO : DAO<UserAddress>
    {
        internal static async Task<List<UserAddress>> FindByUserId(Guid userId)
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

        internal static async Task<UserAddress?> FindById(Guid id)
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
    }
}
