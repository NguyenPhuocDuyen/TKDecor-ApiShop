using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ProductDAO
    {
        public static async Task<List<Product>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var list = await context.Products
                    .OrderByDescending(x => x.UpdatedAt)
                    .Include(x => x.Category)
                    .ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Product> FindById(int id)
        {
            try
            {
                using var context = new TkdecorContext();
                var product = await context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductId == id);
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Product> FindByName(string name)
        {
            try
            {
                using var context = new TkdecorContext();
                var product = await context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Name == name);
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Product> FindBySlug(string slug)
        {
            try
            {
                using var context = new TkdecorContext();
                var product = await context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Slug == slug);
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task Add(Product product)
        {
            try
            {
                using var context = new TkdecorContext();
                await context.AddAsync(product);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task Update(Product product)
        {
            try
            {
                using var context = new TkdecorContext();
                context.Update(product);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
