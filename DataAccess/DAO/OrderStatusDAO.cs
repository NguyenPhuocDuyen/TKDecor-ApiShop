﻿using Bogus.DataSets;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class OrderStatusDAO : DAO<OrderStatus>
    {
        internal static async Task<List<OrderStatus>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.OrderStatuses.ToListAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<OrderStatus?> FindById(Guid id)
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.OrderStatuses
                    .FirstOrDefaultAsync(x => x.OrderStatusId == id);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task<OrderStatus?> FindByName(string name)
        {
            try
            {
                using var context = new TkdecorContext();
                return await context.OrderStatuses
                    .FirstOrDefaultAsync(x => x.Name == name);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
