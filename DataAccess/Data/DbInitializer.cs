using BusinessObject;
using BusinessObject.Other;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly TkdecorContext _db = new();

        public async void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception) { }

            if (_db.Roles.Any()) return;

            List<Role> roles = new()
            {
                new Role { Name = RoleContent.Admin},
                new Role { Name = RoleContent.Seller},
                new Role { Name = RoleContent.Customer},
            };

            _db.Roles.AddRange(roles);
            _db.SaveChanges();

            Role roleAdmin = await _db.Roles.FirstOrDefaultAsync(r => r.Name == RoleContent.Admin);

            User admin = new()
            {
                Email = "admin@admin.com",
                PasswordHash = Password.HashPassword("admin@admin.com"),
                FullName = "admin",
                Role = roleAdmin,
                AvatarUrl = "",
            };
            _db.Users.Add(admin);
            _db.SaveChanges();

            List<CouponType> couponTypes = new()
            {
                new CouponType {Name = CouponTypeContent.ByPercent},
                new CouponType {Name = CouponTypeContent.ByValue},
            };
            _db.CouponTypes.AddRange(couponTypes);
            _db.SaveChanges();

            List<OrderStatus> orders = new()
            {
                new OrderStatus {Name = OrderStatusContent.Ordered},
                new OrderStatus {Name = OrderStatusContent.DeliveringOrders},
                new OrderStatus {Name = OrderStatusContent.OrderReceived},
                new OrderStatus {Name = OrderStatusContent.OrderRefund},
                new OrderStatus {Name = OrderStatusContent.OrderCanceled},
            };
            _db.OrderStatuses.AddRange(orders);
            _db.SaveChanges();

            List<ReportStatus> reportStatuses = new()
            {
                new ReportStatus { Name = ReportStatusContent.Pending},
                new ReportStatus { Name = ReportStatusContent.Accept},
                new ReportStatus { Name = ReportStatusContent.Reject},
            };
            _db.ReportStatuses.AddRange(reportStatuses);
            _db.SaveChanges();
        }
    }
}
