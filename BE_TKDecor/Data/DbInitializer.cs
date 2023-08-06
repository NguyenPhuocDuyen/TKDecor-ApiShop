using Bogus;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly TkdecorContext _db;
        private readonly Random random;

        public DbInitializer(TkdecorContext db)
        {
            _db = db;
            random = new Random();
        }

        public async Task Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception) { }

            //AddModel3D();
            AddUser();
            //await AddArticles();
            AddCategories();
            AddCoupons();
            await AddNotifications();
            await AddProductReports();
            await AddOrders();
            await AddProductReview();
            await AddReportProductReview();
        }

        //private void AddModel3D()
        //{
        //    if (_db.Product3Dmodels.Any()) return;

        //    var mode3dDefault = new Faker<Product3DModel>();
        //    mode3dDefault.RuleFor(x => x.ModelName, f => f.Lorem.Word());
        //    mode3dDefault.RuleFor(x => x.VideoUrl, f => "");
        //    mode3dDefault.RuleFor(x => x.ModelUrl, "https://cdn-luma.com/e13d5b281b9c97e6fbc3defda9a2812bcca09f323705dff6b77646f0d5655dc9.glb");
        //    mode3dDefault.RuleFor(x => x.ThumbnailUrl, "https://fronty.com/static/uploads/01.22-02.22/pexels-uzunov-rostislav-5011647.jpg");

        //    for (var i = 0; i < 4; i++)
        //    {
        //        Product3DModel newModel = mode3dDefault.Generate();
        //        newModel.ModelName += i;
        //        _db.Product3Dmodels.Add(newModel);
        //    }
        //    _db.SaveChanges();
        //}

        private async Task AddReportProductReview()
        {
            if (_db.ReportProductReviews.Any()) return;

            //get one user
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Role == SD.RoleCustomer);

            var reviews = await _db.ProductReviews
                .Include(x => x.Product)
                .ToListAsync();

            List<string> reportStatusValues = new()
            {
                SD.ReportPending,
                SD.ReportAccept,
                SD.ReportReject,
            };

            if (user != null)
            {
                foreach (var review in reviews)
                {
                    string randomStatus = reportStatusValues[random.Next(reportStatusValues.Count)];

                    ReportProductReview reportProductReview = new()
                    {
                        UserReportId = user.UserId,
                        UserReport = user,
                        ProductReviewReportedId = review.ProductReviewId,
                        ProductReviewReported = review,
                        ReportStatus = randomStatus,
                        Reason = "I want to report",
                    };
                    _db.ReportProductReviews.Add(reportProductReview);
                }
                _db.SaveChanges();
            }
        }

        private async Task AddProductReview()
        {
            if (await _db.ProductReviews.AnyAsync()) return;

            var orderDetail = _db.OrderDetails
                .Include(x => x.Order)
                    .ThenInclude(x => x.User)
                .Include(x => x.Product);

            //var productReviewSetDefaults = new Faker<ProductReview>();
            //productReviewSetDefaults.RuleFor(x => x.Description, f => f.Lorem.Paragraph());

            foreach (var od in orderDetail)
            {
                ProductReview productReview = new()
                {
                    UserId = od.Order.UserId,
                    User = od.Order.User,
                    ProductId = od.Product.ProductId,
                    Product = od.Product,
                    Rate = random.Next(3, 6),
                    Description = "Sản phẩm đẹp, chất lượng tốt"
                };
                _db.ProductReviews.Add(productReview);
            }
            _db.SaveChanges();
        }

        private async Task AddProductReports()
        {
            if (_db.ProductReports.Any()) return;

            //var productReportSetDefaults = new Faker<ProductReport>();
            //productReportSetDefaults.RuleFor(x => x.Reason, f => f.Lorem.Paragraph());

            var users = await _db.Users
                .Where(x => x.Role == SD.RoleCustomer)
                .ToListAsync();
            var products = await _db.Products.ToListAsync();

            List<string> reportStatusValues = new()
            {
                SD.ReportPending,
                SD.ReportAccept,
                SD.ReportReject,
            };

            foreach (var u in users)
            {
                foreach (var p in products)
                {
                    string randomStatus = reportStatusValues[random.Next(reportStatusValues.Count)];

                    ProductReport productReport = new()
                    {
                        UserReportId = u.UserId,
                        ProductReportedId = p.ProductId,
                        ReportStatus = randomStatus,
                        Reason = $"Sản phẩm bị lỗi. Yêu cầu kiểm tra lại"
                    };
                    _db.ProductReports.Add(productReport);
                }
                _db.SaveChanges();
            }
        }

        private async Task AddOrders()
        {
            if (_db.Orders.Any()) return;

            var users = await _db.Users
                .Include(x => x.UserAddresses.Where(ud => ud.IsDefault == true))
                .Where(x => x.Role == SD.RoleCustomer)
                .ToListAsync();
            var products = await _db.Products.Take(4).ToListAsync();

            // Lấy danh sách các giá trị enum của OrderStatus
            List<string> orderStatusValues = new()
            {
                SD.OrderOrdered,
                SD.OrderDelivering,
                SD.OrderReceived,
                SD.OrderCanceled
            };

            foreach (var u in users)
            {
                // Random một giá trị từ order status
                string randomStatus = orderStatusValues[random.Next(orderStatusValues.Count)];
                var address = u.UserAddresses?.FirstOrDefault();

                if (address == null) continue;

                Order order = new()
                {
                    UserId = u.UserId,
                    User = u,
                    OrderStatus = randomStatus,
                    FullName = address.FullName,
                    Phone = address.Phone,
                    Address = $"{address.Street}, {address.Ward}, {address.District}, {address.City}",
                    Note = "Hãy giao trước cửa nhà",
                    TotalPrice = 0,
                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var p in products)
                {
                    int randomQuantity = random.Next(2, 5);

                    OrderDetail orderDetail = new()
                    {
                        Order = order,
                        ProductId = p.ProductId,
                        Product = p,
                        Quantity = randomQuantity,
                        PaymentPrice = p.Price
                    };
                    order.TotalPrice += orderDetail.PaymentPrice * orderDetail.Quantity;
                    order.OrderDetails.Add(orderDetail);
                }
                _db.Orders.Add(order);
            }

            _db.SaveChanges();
        }

        private async Task AddNotifications()
        {
            if (_db.Notifications.Any()) return;

            var users = await _db.Users
                .Where(x => x.Role == SD.RoleCustomer)
                .ToListAsync();
            foreach (var u in users)
            {
                Notification notification = new()
                {
                    Message = "Chào mừng bạn tới web TKDecor: ",
                    UserId = u.UserId,
                };
                _db.Notifications.Add(notification);
            }
            _db.SaveChanges();
        }

        private void AddCoupons()
        {
            if (_db.Coupons.Any()) return;

            var couponSetDefault = new Faker<Coupon>();
            couponSetDefault.RuleFor(x => x.IsActive, true);
            couponSetDefault.RuleFor(x => x.RemainingUsageCount, 10);

            for (int i = 1; i < 6; i++)
            {
                Coupon coupon = couponSetDefault.Generate();
                coupon.CouponType = SD.CouponByPercent;
                coupon.Code = ("CodePercent" + i).ToLower();
                coupon.Value = i * 10;
                coupon.MaxValue = 100000;
                _db.Coupons.Add(coupon);
            }

            for (int i = 1; i < 6; i++)
            {
                Coupon coupon = couponSetDefault.Generate();
                coupon.CouponType = SD.CouponByValue;
                coupon.Code = ("CodeValue" + i).ToLower();
                coupon.Value = i * 10000;
                coupon.MaxValue = coupon.Value;
                _db.Coupons.Add(coupon);
            }
            _db.SaveChanges();
        }

        private void AddCategories()
        {
            if (_db.Categories.Any()) return;

            List<Category> categories = new()
            {
                new Category { Name = "Bàn", Thumbnail = "https://icons.veryicon.com/png/o/miscellaneous/common-home-icon/table-26.png",
                    Products = {
                        new Product {
                            Name = "Bàn việc màu trắng",
                            Description = "Bàn làm việc màu trắng đơn giản",
                            Slug = "ban-lam-viec-mau-trang",
                            Quantity = 15,
                            Price = 1200000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/e3/d4/8f/e3d48fd56183f7ca87d6f2ab887ead39.jpg" }
                            }
                        },
                        new Product {
                            Name = "Bàn học thông minh",
                            Description = "Bàn học thông minh có khay đựng sách và đèn LED",
                            Slug = "ban-hoc-thong-minh",
                            Quantity = 100,
                            Price = 1800000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/50/74/aa/5074aa821322c1cf66d20b3032a421eb.jpg" }
                            }
                        },
                        new Product {
                            Name = "Bàn ăn gỗ sồi",
                            Description = "Bàn ăn chất liệu gỗ sồi tự nhiên",
                            Slug = "ban-an-go-soi",
                            Quantity = 80,
                            Price = 2500000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/1e/26/e2/1e26e2867bdcc1f34d35751480ead560.jpg" }
                            }
                        },
                        new Product {
                            Name = "Bàn trà chữ nhật",
                            Description = "Bàn trà chữ nhật với kiểu dáng hiện đại",
                            Slug = "ban-tra-chu-nhat",
                            Quantity = 90,
                            Price = 1800000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/ac/0b/8a/ac0b8adff0a15de6dc85ce66710b5be2.jpg" }
                            }
                        },
                        new Product {
                            Name = "Bàn cà phê gỗ thông",
                            Description = "Bàn cà phê chất liệu gỗ thông tự nhiên",
                            Slug = "ban-ca-phe-go-thong",
                            Quantity = 70,
                            Price = 1500000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/76/0d/35/760d35a345e859e0249b9667f565493a.jpg" }
                            }
                        },
                        new Product {
                            Name = "Bàn trang điểm",
                            Description = "Bàn trang điểm có gương và hộc đựng đồ",
                            Slug = "ban-trang-diem",
                            Quantity = 60,
                            Price = 2000000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/11/02/0f/11020f0873cbd730bbc2bde7c062e4f5.jpg" }
                            }
                        },
                        new Product {
                            Name = "Bàn gỗ thông hình chữ L",
                            Description = "Bàn gỗ thông hình chữ L tiện dụng",
                            Slug = "ban-go-thong-hinh-chu-L",
                            Quantity = 80,
                            Price = 2400000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/736x/51/1e/ab/511eab58d0a6d09bbc14042f98367de1.jpg" }
                            }
                        },
                    }
                },
                new Category { Name = "Ghế", Thumbnail = "https://icons.veryicon.com/png/o/object/articles-for-daily-use/chair-53.png",
                    Products =
                    {
                        new Product
                        {
                            Name = "Ghế xoay văn phòng",
                            Description = "Ghế văn phòng đệm êm, có khung xoay",
                            Slug = "ghe-xoay-van-phong",
                            Quantity = 100,
                            Price = 800000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/9e/4b/a5/9e4ba5904328fb2d2211809b33de7ca3.jpg" },
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/0d/33/2d/0d332d464d5d621960a832e3917576f9.jpg" },
                            }
                        },
                        new Product
                        {
                            Name = "Ghế ăn gỗ",
                            Description = "Ghế ăn chất liệu gỗ cao cấp",
                            Slug = "ghe-an-go",
                            Quantity = 50,
                            Price = 1200000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/78/c4/dd/78c4dd4ff74c714a97e69a9f1c4b546f.jpg" }
                            }
                        },
                    }
                },
                new Category { Name = "Tủ", Thumbnail = "https://icons.veryicon.com/png/o/miscellaneous/common-home-icon/cabinet-15.png",
                    Products =
                    {
                        new Product
                        {
                            Name = "Tủ quần áo 2 cánh",
                            Description = "Tủ quần áo chất liệu gỗ thông",
                            Slug = "tu-quan-ao-2-canh",
                            Quantity = 80,
                            Price = 2200000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/89/a6/ce/89a6ce5091cbb0c431172743ccb8f0b5.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Tủ giày dép đa năng",
                            Description = "Tủ giày đa năng có hộc để đồ",
                            Slug = "tu-giay-dep-da-nang",
                            Quantity = 60,
                            Price = 1100000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/8f/52/9a/8f529afc7075dbcab2bf35ea2f1e62f8.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Tủ sách gỗ sồi",
                            Description = "Tủ sách chất liệu gỗ sồi tự nhiên",
                            Slug = "tu-sach-go-soi",
                            Quantity = 40,
                            Price = 2800000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/e2/cc/df/e2ccdfd6c9d8f38cb34d286656899650.jpg" }
                            }
                        },
                    }
                },
                new Category { Name = "Giường", Thumbnail = "https://icons.veryicon.com/png/o/food--drinks/home-furnishing-furniture/bed-4.png",
                    Products =
                    {
                        new Product
                        {
                            Name = "Giường ngủ đơn",
                            Description = "Giường ngủ đơn chất liệu gỗ thông",
                            Slug = "giuong-ngu-don",
                            Quantity = 50,
                            Price = 1800000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/f0/b0/b8/f0b0b8b1787a79b0184687ffef73bbae.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Giường tầng cho trẻ em",
                            Description = "Giường tầng đa năng cho trẻ em",
                            Slug = "giuong-tang-cho-tre-em",
                            Quantity = 30,
                            Price = 2800000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/50/99/61/5099614a5de0adb5a86c8f433f5aaef6.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Giường sofa đa năng",
                            Description = "Giường sofa có thể biến thành ghế sofa",
                            Slug = "giuong-sofa-da-nang",
                            Quantity = 20,
                            Price = 3500000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/da/5e/60/da5e60a4e2d71eb8b3073578ff039e23.jpg" }
                            }
                        },
                    }
                },
                new Category { Name = "Kệ", Thumbnail = "https://icons.veryicon.com/png/o/system/system-01/shelf-4.png",
                    Products =
                    {
                        new Product
                        {
                            Name = "Kệ sách treo tường",
                            Description = "Kệ sách treo tường đơn giản và tiện ích",
                            Slug = "ke-sach-treo-tuong",
                            Quantity = 90,
                            Price = 900000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/8a/be/f1/8abef1ad8b710770340b6a4095ea209a.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Kệ trang trí 3 tầng",
                            Description = "Kệ trang trí đa năng với 3 tầng",
                            Slug = "ke-trang-tri-3-tang",
                            Quantity = 70,
                            Price = 1300000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/17/1b/9f/171b9febc74f528df1179fad81e30418.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Kệ TV gỗ thông",
                            Description = "Kệ TV chất liệu gỗ thông tự nhiên",
                            Slug = "ke-tv-go-thong",
                            Quantity = 60,
                            Price = 1800000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/c9/cc/36/c9cc36befb56b31e108aea533bdf8866.jpg" }
                            }
                        },
                    }
                },
                new Category { Name = "Đèn", Thumbnail = "https://icons.veryicon.com/png/o/miscellaneous/common-home-icon/lamp-18.png",
                    Products =
                    {
                        new Product
                        {
                            Name = "Đèn bàn màu trắng",
                            Description = "Đèn bàn màu trắng đơn giản",
                            Slug = "den-ban-mau-trang",
                            Quantity = 120,
                            Price = 400000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/57/7b/94/577b9477c80eaba625ccd93b001d9350.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Đèn trần phong cách",
                            Description = "Đèn trần phong cách hiện đại",
                            Slug = "den-tran-phong-cach",
                            Quantity = 80,
                            Price = 1200000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/6b/e9/93/6be99380d41f2b4ef424b981401064d2.jpg" }
                            }
                        },
                    }
                },
                new Category { Name = "Sofa", Thumbnail = "https://icons.veryicon.com/png/o/miscellaneous/common-home-icon/sofa-28.png",
                    Products =
                    {
                        new Product
                        {
                            Name = "Sofa góc chất liệu da",
                            Description = "Sofa góc bọc da cao cấp",
                            Slug = "sofa-goc-chat-lieu-da",
                            Quantity = 50,
                            Price = 3500000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/b3/71/6c/b3716c89c6137fcbbed7dbf9f94c2711.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Sofa góc chất liệu vải",
                            Description = "Sofa góc bọc vải chất liệu cao cấp",
                            Slug = "sofa-goc-chat-lieu-vai",
                            Quantity = 60,
                            Price = 3200000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/1c/e9/46/1ce94614fd0fd235adbbb4a061c8015c.jpg" }
                            }
                        },
                    }
                },
                new Category { Name = "Thảm", Thumbnail = "https://icons.veryicon.com/png/o/miscellaneous/bijing-cloud-chain-common-basic-icons/carpet-2.png",
                    Products =
                    {
                        new Product
                        {
                            Name = "Thảm tròn đan chéo",
                            Description = "Thảm tròn đan chéo màu trắng đen",
                            Slug = "tham-tron-dan-cheo",
                            Quantity = 80,
                            Price = 600000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/d8/3d/f2/d83df2142797dd0c7a7e8a07d571d6ed.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Thảm hình chữ nhật",
                            Description = "Thảm hình chữ nhật màu xanh lá cây",
                            Slug = "tham-hinh-chu-nhat",
                            Quantity = 100,
                            Price = 750000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/aa/c1/51/aac15144857a8764c2d761e6e7d3636f.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Thảm dày mềm mịn",
                            Description = "Thảm dày mềm mịn cho phòng khách",
                            Slug = "tham-day-mem-min",
                            Quantity = 70,
                            Price = 900000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/5d/61/49/5d6149de8bbd4aedb33b067198e3db56.jpg" }
                            }
                        },
                    }
                },
                new Category { Name = "Trang trí", Thumbnail = "https://icons.veryicon.com/png/o/object/niche/ornament-13.png",
                    Products =
                    {
                        new Product
                        {
                            Name = "Tranh treo tường",
                            Description = "Tranh treo tường phong cảnh đẹp",
                            Slug = "tranh-treo-tuong",
                            Quantity = 90,
                            Price = 500000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/73/40/b1/7340b17e8bb8fe2409bb63242809bd03.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Đèn trang trí",
                            Description = "Đèn trang trí đa dạng màu sắc",
                            Slug = "den-trang-tri-bong-kinh",
                            Quantity = 80,
                            Price = 350000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/59/db/66/59db66c4594683ce4367dc5c6fbe9e1c.jpg" }
                            }
                        },
                        new Product
                        {
                            Name = "Gương trang trí hiện đại",
                            Description = "Gương trang trí phong cách hiện đại",
                            Slug = "guong-trang-tri-hien-dai",
                            Quantity = 70,
                            Price = 800000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://i.pinimg.com/564x/58/e0/b5/58e0b5b209d6b7a4bf5f711b7c051132.jpg" }
                            }
                        },
                    }
                }
            };
            _db.Categories.AddRange(categories);
            _db.SaveChanges();
        }

        //private async Task AddArticles()
        //{
        //    if (_db.Articles.Any()) return;

        //    var user = await _db.Users.FirstOrDefaultAsync(x => x.Role == Role.Admin);
        //    if (user != null)
        //    {
        //        var articleSetDefaults = new Faker<Article>();
        //        articleSetDefaults.RuleFor(x => x.User, user);
        //        articleSetDefaults.RuleFor(x => x.Title, f => f.Lorem.Sentence());
        //        articleSetDefaults.RuleFor(x => x.Content, f => f.Lorem.Paragraphs());
        //        articleSetDefaults.RuleFor(x => x.Thumbnail, "https://kinsta.com/wp-content/uploads/2020/08/tiger-jpg.jpg");
        //        articleSetDefaults.RuleFor(x => x.IsPublish, true);
        //        for (var i = 0; i < 20; i++)
        //        {
        //            Article article = articleSetDefaults.Generate();
        //            article.Title = "Article 1: " + article.Title;
        //            article.Slug = Slug.GenerateSlug(article.Title);
        //            _db.Articles.Add(article);
        //        }
        //        _db.SaveChanges();
        //    }
        //}

        private void AddUser()
        {
            if (_db.Users.Any()) return;

            // add admin
            User admin = new()
            {
                Email = "admin@gmail.com",
                Password = Password.HashPassword("Default@123"),
                FullName = "admin",
                Role = SD.RoleAdmin,
                BirthDay = DateTime.Now,
                Gender = SD.GenderMale,
                Phone = GenerateRandomPhoneNumber(),
                EmailConfirmed = true,
                AvatarUrl = "https://static.vecteezy.com/system/resources/previews/000/439/863/original/vector-users-icon.jpg",
            };
            _db.Users.Add(admin);

            var userSetDefaults = new Faker<User>();
            userSetDefaults.RuleFor(a => a.Email, "@gmail.com");
            userSetDefaults.RuleFor(a => a.Password, Password.HashPassword("Default@123"));
            userSetDefaults.RuleFor(a => a.FullName, f => f.Lorem.Word());
            userSetDefaults.RuleFor(a => a.EmailConfirmed, true);
            userSetDefaults.RuleFor(a => a.AvatarUrl, "https://static.vecteezy.com/system/resources/previews/000/439/863/original/vector-users-icon.jpg");

            var userAddressSetDefaults = new Faker<UserAddress>();
            //userAddressSetDefaults.RuleFor(x => x.Address, f => f.Lorem.Sentence());
            userAddressSetDefaults.RuleFor(x => x.CityCode, 1);
            userAddressSetDefaults.RuleFor(x => x.City, "Thành phố Hà Nội");
            userAddressSetDefaults.RuleFor(x => x.DistrictCode, 1);
            userAddressSetDefaults.RuleFor(x => x.District, "Quận Ba Đình");
            userAddressSetDefaults.RuleFor(x => x.WardCode, 1);
            userAddressSetDefaults.RuleFor(x => x.Ward, "Phường Phúc Xá");
            userAddressSetDefaults.RuleFor(x => x.Street, f => "324 hẻm 6 " + f.Lorem.Sentence());

            // Lấy danh sách các giá trị enum của OrderStatus
            List<string> genderValues = new()
            {
                SD.GenderMale,
                SD.GenderFemale,
                SD.GenderOther,
            };

            // add customer and seller
            //Role? roleCustomer = await _db.Roles.FirstOrDefaultAsync(r => r.Name == Role.Customer);
            for (int i = 0; i < 15; i++)
            {
                string randomStatus = genderValues[random.Next(genderValues.Count)];
                User u = userSetDefaults.Generate();
                u.Role = SD.RoleCustomer;
                u.Email = $"Customer{i}{u.Email}".ToLower();
                u.FullName = $"Customer {i} {u.FullName}";
                u.UserAddresses = new List<UserAddress>();
                u.BirthDay = DateTime.Now;
                u.Gender = randomStatus;
                u.Phone = GenerateRandomPhoneNumber();

                for (int j = 0; j < 2; j++)
                {
                    UserAddress address = userAddressSetDefaults.Generate();
                    address.Phone = GenerateRandomPhoneNumber();
                    address.User = u;
                    address.FullName = u.FullName;

                    if (j == 0) address.IsDefault = true;
                    u.UserAddresses.Add(address);
                }
                _db.Users.Add(u);
            }
            _db.SaveChanges();
        }

        private static string GenerateRandomPhoneNumber()
        {
            Random random = new();
            string phoneNumber = "0";

            // Thêm 10 chữ số ngẫu nhiên
            for (int i = 0; i < 9; i++)
            {
                phoneNumber += random.Next(0, 10);
            }

            return phoneNumber;
        }
    }
}
