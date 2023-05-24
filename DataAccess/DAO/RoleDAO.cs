using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class RoleDAO
    {
        public static async Task<Role> FindByName(string name)
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
