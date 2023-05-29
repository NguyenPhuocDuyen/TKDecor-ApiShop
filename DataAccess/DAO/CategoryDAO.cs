using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CategoryDAO
    {
        public static async Task<List<Category>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var listCategories =await context.Categories.ToListAsync();
                return listCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
