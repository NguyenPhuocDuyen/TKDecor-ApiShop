﻿using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    internal class OrderDetailDAO
    {
        internal static async Task<OrderDetail?> FindByUserIdAndProductId(Guid userId, Guid productId)
        {
            try
            {
                using var context = new TkdecorContext();
                var orderDetail = await context.OrderDetails
                    .Include(x => x.Order)
                    .FirstOrDefaultAsync(x =>
                        x.ProductId == productId
                        && x.Order.UserId == userId);
                return orderDetail;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
