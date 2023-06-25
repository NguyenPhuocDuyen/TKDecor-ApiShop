using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class RoleDAO
    {
        internal static async Task<Role?> FindByName(string name)
        {
            Role? role;
            try
            {
                using var context = new TkdecorContext();
                role = await context.Roles.Where(r => r.Name == name).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return role;
        }
    }
}
