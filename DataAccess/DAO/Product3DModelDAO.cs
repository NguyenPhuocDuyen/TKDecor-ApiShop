using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    internal class Product3DModelDAO : DAO<Product3DModel>
    {
        internal static async Task<List<Product3DModel>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var models = await context.Product3Dmodels
                    .Include(x => x.Product)
                    .ToListAsync();
                return models;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<Product3DModel?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                var model = await context.Product3Dmodels
                    .Include(x => x.Product)
                    .FirstOrDefaultAsync(x => x.Product3DModelId == id);
                return model;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
