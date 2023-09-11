using Bogus;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
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

            //var admin = _db.Users.Where(x => x.Email == "admin2@gmail.com");
            //_db.RemoveRange(admin);
            //_db.SaveChanges();

            // add admin
            //User admin = new()
            //{
            //    Email = "admin2@gmail.com",
            //    Password = Password.HashPassword("Default@123"),
            //    FullName = "admin 2",
            //    Role = SD.RoleAdmin,
            //    BirthDay = DateTime.Now,
            //    Gender = SD.GenderMale,
            //    Phone = GenerateRandomPhoneNumber(),
            //    EmailConfirmed = true,
            //    AvatarUrl = "https://static.vecteezy.com/system/resources/previews/000/439/863/original/vector-users-icon.jpg",
            //};
            //_db.Users.Add(admin);
            //_db.SaveChanges();

            ////DeleteData();

            AddModel3D();
            AddUser();
            await AddArticles();
            AddCategories();
            //AddCoupons();
            //await AddNotifications();
            //await AddProductReports();
            //await AddOrders();
            //await AddProductReview();
            //await AddReportProductReview();
        }

        private void DeleteData()
        {
            var productReports = _db.ProductReports.ToList();
            _db.ProductReports.RemoveRange(productReports);
            _db.SaveChanges();

            var reportProductReview = _db.ReportProductReviews.ToList();
            _db.ReportProductReviews.RemoveRange(reportProductReview);
            _db.SaveChanges();

            var interaction = _db.ProductReviewInteractions.ToList();
            _db.ProductReviewInteractions.RemoveRange(interaction);
            _db.SaveChanges();

            var ods = _db.OrderDetails.Include(x => x.ProductReview).ToList();
            foreach (var od in ods)
            {
                od.ProductReview = null;
                od.ProductReviewId = null;
                _db.OrderDetails.Update(od);
            }
            _db.SaveChanges();

            var reviews = _db.ProductReviews.ToList();
            _db.ProductReviews.RemoveRange(reviews);
            _db.SaveChanges();

            _db.OrderDetails.RemoveRange(ods);
            _db.SaveChanges();

            var orders = _db.Orders.ToList();
            _db.Orders.RemoveRange(orders);
            _db.SaveChanges();
        }

        private void AddModel3D()
        {
            if (_db.Product3Dmodels.Any()) return;

            //var mode3dDefault = new Faker<Product3DModel>();
            //mode3dDefault.RuleFor(x => x.ModelName, f => f.Lorem.Word());
            //mode3dDefault.RuleFor(x => x.VideoUrl, f => "");
            //mode3dDefault.RuleFor(x => x.ModelUrl, "https://cdn-luma.com/e13d5b281b9c97e6fbc3defda9a2812bcca09f323705dff6b77646f0d5655dc9.glb");
            //mode3dDefault.RuleFor(x => x.ThumbnailUrl, "https://fronty.com/static/uploads/01.22-02.22/pexels-uzunov-rostislav-5011647.jpg");

            //for (var i = 0; i < 4; i++)
            //{
            //    Product3DModel newModel = mode3dDefault.Generate();
            //    newModel.ModelName += i;
            //    _db.Product3Dmodels.Add(newModel);
            //}
            //_db.SaveChanges();

            var list = new List<Product3DModel>(){
                new Product3DModel { ModelName = "Ghế ngồi 4 chân",
                    VideoUrl = "https://cdn-luma.com/c04424894fa366b6fdc1946c8bd0e1e5b33430c056bb67b6cc4c277b11dab464/TKDecor_with_background.mp4",
                    ModelUrl = "https://cdn-luma.com/e3abb05cdbbf539c08b5f875139e8982c21b6ba808313548d0c02f08532ab330/TKDecor_textured_mesh_glb.glb",
                    ThumbnailUrl = "https://cdn-luma.com/2cf873cb6046036fdc1e53eadbd8bb9a3c044a84672245ae03123fcdd3265187/TKDecor_thumb.jpg" },
                new Product3DModel { ModelName = "Sofa trắng",
                    VideoUrl = "https://cdn-luma.com/09c546945dda0a49b50a6032621cf547a04fc55f3d9a1934dd3b876d9db01094/TKDecor_with_background.mp4",
                    ModelUrl = "https://cdn-luma.com/eac7e0a3f71f4c322c79036bbf59620523f19fce373ecbb1af0188631bc4b765/TKDecor_textured_mesh_glb.glb",
                    ThumbnailUrl = "https://cdn-luma.com/d4128aaac4a1add70c69570db5e2f8cedf19f6b9461f054526434088ef896260/TKDecor_thumb.jpg" },
                new Product3DModel { ModelName = "Bình Cây Xanh",
                    VideoUrl = "https://cdn-luma.com/4c6719ec5f3375b5991101586c1fca4e41b2ee329100349eab9d23d49beda0da/TKDecor_with_background.mp4",
                    ModelUrl = "https://cdn-luma.com/3bea3f46662c539a154a9ebc45204c930e0da70b258c0917e0faa368f7848159/TKDecor_textured_mesh_glb.glb",
                    ThumbnailUrl = "https://cdn-luma.com/d1ffced67d2f5bb369665d4e08c052025f5ba50a8758c02f5b156e1055b35068/TKDecor_thumb.jpg" },
                new Product3DModel { ModelName = "Ghế sắt",
                    VideoUrl = "https://cdn-luma.com/e3672534cb91d10cb3675fd52986d3afd39f84e155f996ec39f5019047239b09/TKDecor_with_background.mp4",
                    ModelUrl = "https://cdn-luma.com/96ea38bf8e1b9210056a8bc0da8acb366fd72e6ffe6c41b6b121091ca43b934f/TKDecor_textured_mesh_glb.glb",
                    ThumbnailUrl = "https://cdn-luma.com/a94d6d62a41ba3119025c33e8550984d21aaba5ade61b069ba8ccc0216ef3310/TKDecor_thumb.jpg" },
                new Product3DModel { ModelName = "Đèn ngủ nghệ thuật",
                    VideoUrl = "https://cdn-luma.com/0d09aead2381965771b7b776d22059c80d5e7e1fc58ecf17d16f0fe5ce0332f0/TKDecor_with_background.mp4",
                    ModelUrl = "https://cdn-luma.com/e8ec434535cef175fbd2c674128ac53e366527c13dccfaba944778c647f1a96b/TKDecor_textured_mesh_glb.glb",
                    ThumbnailUrl = "https://cdn-luma.com/0f053aef5bc3afa41757cb9e2ecdb86ed03c66c7921b7e8af13f8e770514f99c/TKDecor_thumb.jpg" },
                new Product3DModel { ModelName = "Bàn gỗ",
                    VideoUrl = "https://cdn-luma.com/89a79cb6bb3dc35d3c95d8bca6e1aaa9c10074f9d82d48bc60f00f929aabefa4/TKDecor_with_background.mp4",
                    ModelUrl = "https://cdn-luma.com/b897032d2ac22151d19b9aeef0ef8fbc044f8a7151d8a5580a3528b8c0626faf/TKDecor_textured_mesh_glb.glb",
                    ThumbnailUrl = "https://cdn-luma.com/dd4ec9ae41f3019c9008e37a0459642afedb08be3236a46d4925e430b37a29e3/TKDecor_thumb.jpg" },
                new Product3DModel { ModelName = "Ghế đơn da",
                    VideoUrl = "https://cdn-luma.com/2b37000ec6b5d0e7f0ed82ddca113ddc1e90101e1ec6cd6a4a1314325e7c0bbd/TKDecor_with_background.mp4",
                    ModelUrl = "https://cdn-luma.com/6d5cc694c12db7fd1a28161c7cf3414dc59a537703ba87a9aabe30c642fc29eb/TKDecor_textured_mesh_glb.glb",
                    ThumbnailUrl = "https://cdn-luma.com/cbb835ffa921eca62fd86fb57aa01882082b49a44afd6a258b40d8f08f09dbe4/TKDecor_thumb.jpg" },
                new Product3DModel { ModelName = "Ghế sofa da trơn",
                    VideoUrl = "https://cdn-luma.com/c4ca49a20cd4d2fccc3d66445a8bb0c637c558e9c290b3ac47e19928d823dcb1/TKDecor_with_background.mp4",
                    ModelUrl = "https://cdn-luma.com/8684fd9f7a5551081ba827b9351121471fc906f4b56aa52602c2e2e822d1af7a/TKDecor_textured_mesh_glb.glb",
                    ThumbnailUrl = "https://cdn-luma.com/5890de44f8877fe635d4091ea927cca404295a559e1b1a334a85d8984132d0ec/TKDecor_thumb.jpg" },
                new Product3DModel { ModelName = "Giường bọc vải màu xám Cavil",
                    VideoUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/video%2Fvideo_tkdecor.mp4?alt=media&token=9515020e-ce72-4b14-aabe-2f8924589806",
                    ModelUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/images%2Fe7551bba-0827-4ac3-bce0-3f16a98f2d77.glb?alt=media&token=893a5276-718c-4659-8407-777d0a3c1bb1",
                    ThumbnailUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/images%2F83438c8c-487a-41e0-86e5-bae8e0f438d9.?alt=media&token=7d4bc006-ab27-4df3-8e5f-d0cd95829708" },
                new Product3DModel { ModelName = "Ghế sofa M8",
                    VideoUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/video%2Fvideo_tkdecor.mp4?alt=media&token=9515020e-ce72-4b14-aabe-2f8924589806",
                    ModelUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/images%2Fe78cb7d2-e705-4a73-b0b9-4facc71461ee.glb?alt=media&token=a3d56a82-719a-4aad-84a1-975aa918979f",
                    ThumbnailUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/images%2F140b0126-06f9-461b-989a-386a9747221b.?alt=media&token=f4697f3b-b999-403b-88de-591c9a21f6bb" },
                new Product3DModel { ModelName = "Bàn gỗ phương Đông Trung Quốc",
                    VideoUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/video%2Fvideo_tkdecor.mp4?alt=media&token=9515020e-ce72-4b14-aabe-2f8924589806",
                    ModelUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/images%2F4cad958b-3d9d-4a05-a1f8-6eb431552359.glb?alt=media&token=4340993d-0f35-44fb-9d3e-458f81dab52c",
                    ThumbnailUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/images%2F0c40686c-4c0c-49d5-9912-48d2a58679c1.?alt=media&token=0897eb5a-c30f-4f19-97b2-1fb4705c12a0" },
                new Product3DModel { ModelName = "Ghế treo ngoài trời MAY BRISA",
                    VideoUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/video%2Fvideo_tkdecor.mp4?alt=media&token=9515020e-ce72-4b14-aabe-2f8924589806",
                    ModelUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/images%2F30e592dd-eb39-447a-aecc-8d1e112cab77.glb?alt=media&token=904b3858-1fb7-4625-9ef5-aa70e8593c2d",
                    ThumbnailUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/images%2Fbab80495-1b68-45d7-be2f-d7df694605df.?alt=media&token=52c7eb5c-ffbc-43c5-a533-bf6a7c9f5a7d" },
                new Product3DModel { ModelName = "Sofa chữ L SL40",
                    VideoUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/video%2Fvideo_tkdecor.mp4?alt=media&token=9515020e-ce72-4b14-aabe-2f8924589806",
                    ModelUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/images%2Fb5b1cd4d-4efe-4fff-b169-9ad3b81637fa.glb?alt=media&token=3d7d8c1a-1627-4124-bc94-1af9ed2214f7",
                    ThumbnailUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/images%2Fe6b3b238-8416-4ebc-b752-b06dc1f959d9.?alt=media&token=a6cb7bc0-bece-4292-8f07-e13489d7fe29" },
            };

            _db.Product3Dmodels.AddRange(list);
            _db.SaveChanges();
        }

        private async Task AddReportProductReview()
        {
            if (_db.ReportProductReviews.Any()) return;

            //get one user
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Role == SD.RoleCustomer);

            var reviews = await _db.ProductReviews
                //.Include(x => x.Product)
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
                        Reason = "Tôi muốn báo cáo đánh giá này",
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

            foreach (var od in orderDetail)
            {
                ProductReview productReview = new()
                {
                    Rate = random.Next(3, 6),
                    Description = "Sản phẩm đẹp, chất lượng tốt"
                };
                od.ProductReview = productReview;
                _db.OrderDetails.Update(od);
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
                        new Product {
                            Name = "Bàn gỗ phương Đông Trung Quốc",
                            Description = "<p>Bàn Gỗ Phương Đông Trung Quốc là một tuyệt tác nghệ thuật kết hợp giữa chất liệu gỗ tự nhiên cao cấp và sự tinh tế trong thiết kế, tạo nên một món đồ nội thất đẹp mắt và sang trọng cho không gian sống của bạn. Được chế tạo tại các làng nghề truyền thống ở vùng Đông Á, sản phẩm này thể hiện tinh hoa của nghệ thuật gỗ truyền thống Trung Quốc.</p><p>Đặc điểm nổi bật:</p><ol><li>Chất liệu gỗ tự nhiên: Bàn được làm từ gỗ tự nhiên cao cấp, giúp sản phẩm có độ bền cao và vẻ đẹp tự nhiên. Mỗi chi tiết gỗ đều được chăm sóc và tạo hình thủ công tinh xảo.</li><li>Thiết kế độc đáo: Với sự kết hợp giữa yếu tố truyền thống và hiện đại, Bàn Gỗ Phương Đông Trung Quốc có một thiết kế độc đáo và đầy quyến rũ. Các họa tiết truyền thống Trung Quốc, như hoa văn và hình ảnh phượng hoàng, thường được khắc hoặc vẽ trên bề mặt bàn.</li><li>Tích hợp nghệ thuật và sử dụng: Sản phẩm không chỉ là một món đồ nội thất mà còn là một tác phẩm nghệ thuật. Bàn Gỗ Phương Đông Trung Quốc có thể được đặt trong phòng khách, phòng làm việc, hoặc phòng ngủ, làm điểm nhấn cho không gian nội thất của bạn.</li><li>Lựa chọn đa dạng: Sản phẩm được cung cấp với nhiều kích thước và màu sắc khác nhau để phù hợp với nhu cầu và sở thích của từng khách hàng.</li><li>Chất lượng đỉnh cao: Sản phẩm được sản xuất bằng quy trình kiểm tra chất lượng nghiêm ngặt, đảm bảo rằng bạn đang sở hữu một món đồ nội thất bền bỉ và đẳng cấp.</li></ol>",
                            Slug = "ban-go-phuong-dong-trung-quoc",
                            Quantity = 10,
                            Price = 2550000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2Ff004e4e9-7063-4da8-a440-9264736d4ea7%2Fchina-table.jpg?alt=media&token=8e8a312a-5156-4db3-af68-fa274125c739" }
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
                        new Product
                        {
                            Name = "Ghế sofa M8",
                            Description = "<p>Sofa M8 - Sự Kết Hợp Hoàn Hảo Giữa Phong Cách và Tiết Kiệm</p><p><br></p><p>Sofa M8 là một giải pháp tuyệt vời cho những người đang tìm kiếm một chiếc ghế sofa chất lượng với giá cả phải chăng. Với thiết kế hiện đại và sự thoải mái, đây là sự kết hợp hoàn hảo giữa phong cách và tiết kiệm. Dưới đây là một số điểm nổi bật về sản phẩm này:</p><p><br></p><p>1. **Thiết Kế Hiện Đại**: Sofa M8 có một thiết kế đơn giản và hiện đại, phù hợp với nhiều phong cách trang trí nội thất khác nhau. Với các đường nét tinh tế và tỉ mỉ trong thiết kế, nó sẽ làm cho phòng khách của bạn trở nên sang trọng và thu hút.</p><p><br></p><p>2. **Chất Lượng Tốt**: Mặc dù có giá rẻ, nhưng sofa này được làm từ các chất liệu chất lượng, đảm bảo độ bền và sự thoải mái cho người sử dụng. Khung bằng gỗ chắc chắn kết hợp với lớp nệm êm ái giúp tạo cảm giác thoải mái khi ngồi.</p><p><br></p><p>3. **Lựa Chọn Màu Sắc Đa Dạng**: Sofa M8 có sẵn trong nhiều tùy chọn màu sắc khác nhau, cho phép bạn lựa chọn màu phù hợp với trang trí và sở thích cá nhân của bạn.</p><p><br></p><p>4. **Kích Thước Phù Hợp**: Sản phẩm có sẵn trong các kích thước khác nhau, từ 2 chỗ đến 3 chỗ, giúp bạn dễ dàng tùy chỉnh để phù hợp với không gian của bạn.</p><p><br></p><p>5. **Giá Cả Phải Chăng**: Điều quan trọng là Sofa M8 có giá cả phải chăng, là lựa chọn tốt cho những người muốn có một chiếc ghế sofa đẹp mà không cần phải chi trả quá nhiều.</p><p><br></p><p>Với Sofa M8, bạn sẽ có cơ hội thư giãn và tận hưởng không gian sống của mình mà không phải lo lắng về ngân sách. Hãy tận dụng cơ hội này để thêm một chiếc sofa thú vị vào ngôi nhà của bạn.</p>",
                            Slug = "ghe-sofa-m8",
                            Quantity = 15,
                            Price = 6900000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2Ffcd20773-e5b3-4ccf-8023-4c2312c6e206%2Fsofa-modern-23.jpeg?alt=media&token=163995de-365c-4f3e-a65f-502cd26e4652" },
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2Ffcd20773-e5b3-4ccf-8023-4c2312c6e206%2Fsofa-modern-2.jpeg?alt=media&token=2f8f8a80-cf3e-4dec-9cbc-5b8b6dcae26d" },
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2Ffcd20773-e5b3-4ccf-8023-4c2312c6e206%2Fsofa-modern.jpeg?alt=media&token=1e5c3b2b-e8bb-44cc-96cc-d3a932fb6744" }
                            }
                        },
                        new Product
                        {
                            Name = "Ghế treo ngoài trời MAY BRISA",
                            Description = "<p>Ghế Treo Ngoài Trời May Brisa - Tận hưởng Bầu Trời và Thiên Nhiên</p><p><br></p><p>Ghế Treo Ngoài Trời May Brisa là một sản phẩm ngoại trời tuyệt vời để bạn có thể thư giãn và tận hưởng không gian ngoài trời một cách thoải mái và thú vị. Dưới đây là một số điểm nổi bật về sản phẩm này:</p><p><br></p><p>1. **Thiết Kế Đẹp Mắt**: Ghế Treo Ngoài Trời May Brisa có thiết kế hiện đại và thanh lịch. Với khung ghế bằng thép chắc chắn và đệm bọc vải chất lượng, nó là sự kết hợp hoàn hảo giữa phong cách và sự thoải mái.</p><p><br></p><p>2. **Chất Liệu Chất Lượng Cao**: Sản phẩm được làm từ chất liệu chống thời tiết, chịu được nắng mưa, giúp giữ cho ghế luôn mới mẻ và bền bỉ trong môi trường ngoại trời khắc nghiệt.</p><p><br></p><p>3. **Thoải Mái và Thư Giãn**: Ghế Treo Ngoài Trời May Brisa có thiết kế treo, giúp bạn có thể nằm thư giãn và tận hưởng bầu trời và thiên nhiên một cách thoải mái. Đệm êm ái giúp bạn cảm thấy thư giãn hơn bao giờ hết.</p><p><br></p><p>4. **Dễ Lắp Đặt**: Sản phẩm đi kèm với hệ thống treo tiện lợi, giúp bạn dễ dàng lắp đặt và di chuyển ghế theo ý muốn.</p><p><br></p><p>5. **Phù Hợp Với Nhiều Không Gian**: Ghế Treo Ngoài Trời May Brisa phù hợp với nhiều không gian ngoại trời, từ ban công nhỏ, sân vườn, đến bãi biển và khu resort.</p><p><br></p><p>6. **Giá Trị Đáng Đầu Tư**: Dù có giá cao hơn so với một số sản phẩm ngoại trời khác, nhưng Ghế Treo Ngoài Trời May Brisa mang lại giá trị đáng đầu tư bởi sự kết hợp giữa thiết kế đẹp và chất lượng.</p><p><br></p><p>Với Ghế Treo Ngoài Trời May Brisa, bạn có thể biến không gian ngoại trời của mình thành một nơi thư giãn tuyệt vời để tận hưởng không khí trong lành và thoải mái. Sản phẩm này là sự lựa chọn hoàn hảo cho những ai yêu thích cuộc sống ngoài trời và muốn tạo ra một góc thư giãn độc đáo.</p>",
                            Slug = "ghe-treo-ngoai-troi-may-brisa",
                            Quantity = 10,
                            Price = 26000000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2F899c0ec2-692a-418f-8f74-bd947f430e00%2Foutdoor-chair-2.jpg?alt=media&token=5e46b9b2-33a9-4a07-aecb-7b1a4b70854b" },
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2F899c0ec2-692a-418f-8f74-bd947f430e00%2Foutdoor-chair.jpg?alt=media&token=7833c844-4dfa-46c9-9fa4-59a15734b360" },
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2F899c0ec2-692a-418f-8f74-bd947f430e00%2Foutdoor-chair-1.jpg?alt=media&token=8e88fd8e-92a8-4cc2-81ed-77b9078f2044" }
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
                        new Product
                        {
                            Name = "Giường bọc vải màu xám Cavil",
                            Description = "<p>Giường bọc vải màu xám Cavil - Sự Kết Hợp Đẹp Mắt Giữa Phong Cách và Thoải Mái</p><p><br></p><p>Giường bọc vải màu xám Cavil là một sản phẩm nội thất tuyệt vời cho phòng ngủ của bạn. Với sự kết hợp hoàn hảo giữa phong cách và thoải mái, nó sẽ là một điểm nhấn đẹp mắt trong không gian nghỉ ngơi của bạn. Dưới đây là một số điểm nổi bật về sản phẩm này:</p><p><br></p><p>1. <strong>Thiết Kế Sang Trọng</strong>: Giường Cavil có thiết kế đơn giản, nhưng đầy tính thẩm mỹ với màu xám tối trẻ trung và hiện đại. Bề mặt giường bọc vải mềm mại tạo cảm giác thoải mái và ấm áp.</p><p><br></p><p>2. <strong>Chất Lượng Chất Lượng</strong>: Sản phẩm được làm từ chất liệu chất lượng cao, đảm bảo độ bền và sự thoải mái khi sử dụng. Khung giường chắc chắn và độ bền của vải đảm bảo rằng bạn sẽ sử dụng được sản phẩm trong thời gian dài.</p><p><br></p><p>3. <strong>Kích Thước Đa Dạng</strong>: Giường Cavil có sẵn trong nhiều kích thước khác nhau, từ đơn đến king size, giúp bạn dễ dàng tùy chỉnh để phù hợp với không gian và nhu cầu của bạn.</p><p><br></p><p>4. <strong>Lựa Chọn Màu Sắc</strong>: Bên cạnh màu xám, sản phẩm này còn có thể có sẵn trong một loạt các màu sắc khác nhau, cho phép bạn lựa chọn màu ưa thích để phù hợp với trang trí nội thất của phòng ngủ.</p><p><br></p><p>5. <strong>Không Gian Lưu Trữ</strong>: Một số phiên bản của giường Cavil có khả năng lưu trữ bên dưới giường, giúp bạn tiết kiệm không gian và tận dụng không gian lưu trữ một cách hiệu quả.</p><p><br></p><p>6. <strong>Giá Cả Hợp Lý</strong>: Đặc biệt là so với chất lượng và thiết kế, giường Cavil có giá cả hợp lý, là sự đầu tư tốt cho phòng ngủ của bạn.</p><p><br></p><p>Với Giường bọc vải màu xám Cavil, bạn sẽ có không gian ngủ lý tưởng để thư giãn và tận hưởng giấc ngủ ngon mỗi đêm. Thiết kế đẹp mắt và chất lượng vượt trội của sản phẩm này sẽ làm cho phòng ngủ của bạn trở nên sang trọng và thoải mái.</p>",
                            Slug = "giuong-boc-vai-mau-xam-cavil",
                            Quantity = 20,
                            Price = 6500000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2Fa2a1f206-2a43-4da7-8dce-34054c8b4f79%2Fdouble-bed-1.png?alt=media&token=d7e2bc9b-c0ef-4241-b19f-8ace6c17d790" },
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2Fa2a1f206-2a43-4da7-8dce-34054c8b4f79%2Fdouble-bed-2.png?alt=media&token=9f7bb05f-2b88-41dd-a747-b5b1f49afbd2" },
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2Fa2a1f206-2a43-4da7-8dce-34054c8b4f79%2Fdouble-bed.png?alt=media&token=f50a40f8-bb19-4b25-84e7-cf5ff2aacb64" },
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-v2.appspot.com/o/products%2Fa2a1f206-2a43-4da7-8dce-34054c8b4f79%2Fdouble-bed-4.png?alt=media&token=cd76c32c-a0af-4d55-9ee5-6c0ae5201e60" },
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
                        new Product
                        {
                            Name = "Sofa trắng sáng",
                            Description = "<p>Sofa giống như mô tả nó đẹp lắm.</p><p>Nó sử dụng chất liệu thượng hạng đến từ các thương hiệu nổi tiếng và đình dám nhất trên thế giới phải gọi là tinh hoa hội tụ, người người rất yêu, nó giống như nàng Mona Lisa của giới đồ nội thất</p>",
                            Slug = "sofa-trang-sang",
                            Quantity = 197,
                            Price = 10000000,
                            ProductImages =
                            {
                                new ProductImage { ImageUrl = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/products%2F70f5bc17-84d0-416a-aef8-9c2396129259%2Fsofa_trang.jpg?alt=media&token=e7e9e789-0e8d-421f-b8fa-077502a36a5c" }
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

        private async Task AddArticles()
        {
            if (_db.Articles.Any()) return;

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Role == SD.RoleAdmin);
            if (user != null)
            {
                //var articleSetDefaults = new Faker<Article>();
                //articleSetDefaults.RuleFor(x => x.User, user);
                //articleSetDefaults.RuleFor(x => x.Title, f => f.Lorem.Sentence());
                //articleSetDefaults.RuleFor(x => x.Content, f => f.Lorem.Paragraphs());
                //articleSetDefaults.RuleFor(x => x.Thumbnail, "https://kinsta.com/wp-content/uploads/2020/08/tiger-jpg.jpg");
                //articleSetDefaults.RuleFor(x => x.IsPublish, true);
                //for (var i = 0; i < 2; i++)
                //{
                //    Article article = articleSetDefaults.Generate();
                //    article.Title = "Article 1: " + article.Title;
                //    article.Slug = Slug.GenerateSlug(article.Title);
                //    _db.Articles.Add(article);
                //}
                //_db.SaveChanges();
                var list = new List<Article>()
                {
                    new Article { UserId = user.UserId,
                    Title = "Tiêu chuẩn chất lượng trong thiết kế và lựa chọn đồ nội thất",
                    Content = "<p>Tiêu chuẩn chất lượng trong thiết kế và lựa chọn đồ nội thất</p><p>Đồ nội thất đóng một vai trò quan trọng trong việc tạo nên không gian sống đẹp mắt, thoải mái và tiện nghi. Không chỉ là những món đồ để sắp xếp, nội thất còn phản ánh phong cách sống, gu thẩm mỹ và tính cách của chủ nhân. Trong bài viết này, chúng ta sẽ khám phá về tầm quan trọng của đồ nội thất, tiêu chuẩn chất lượng cần xem xét và một số xu hướng thiết kế nội thất đang thịnh hành.</p><p>I. Tầm quan trọng của đồ nội thất</p><ol><li>Tạo không gian thoải mái: Đồ nội thất giúp tạo ra môi trường sống thoải mái, nơi mà bạn và gia đình có thể thư giãn sau một ngày làm việc căng thẳng.</li><li>Phản ánh cá nhân: Lựa chọn nội thất thể hiện cá tính, gu thẩm mỹ và sở thích của chủ nhân. Từ màu sắc, kiểu dáng cho đến chất liệu, tất cả đều tạo nên dấu ấn riêng của không gian.</li><li>Tối ưu hóa không gian: Đồ nội thất thông minh có thể giúp tối ưu hóa diện tích căn phòng, đặc biệt là trong những căn hộ có diện tích nhỏ hẹp.</li><li>Hỗ trợ chức năng: Điều quan trọng là đồ nội thất không chỉ đẹp mắt mà còn phải phục vụ tốt các chức năng sử dụng, chẳng hạn như bàn ăn tiện lợi, giường êm ái, ghế làm việc ergonomics, v.v.</li></ol><p>II. Tiêu chuẩn chất lượng trong thiết kế và lựa chọn đồ nội thất</p><ol><li>Chất liệu: Chất liệu đóng vai trò quan trọng trong việc quyết định về độ bền, đẹp và an toàn của đồ nội thất. Gỗ tự nhiên thường là lựa chọn phổ biến với độ bền cao và vẻ đẹp tự nhiên. Các vật liệu như kim loại, da, nhựa cũng có thể được sử dụng phù hợp với từng mục đích sử dụng.</li><li>Thiết kế và kiểu dáng: Thiết kế của đồ nội thất nên phản ánh tính thẩm mỹ và chức năng của sản phẩm. Kiểu dáng phải hài hòa, không gây cảm giác lỗi thời và phải phù hợp với không gian sử dụng.</li><li>Ergonomics (nguyên tắc thiết kế dựa trên sự thoải mái và hiệu suất): Để đảm bảo sự thoải mái và sức khỏe cho người sử dụng, nguyên tắc ergonomics cần được áp dụng trong thiết kế đồ nội thất. Ghế, bàn làm việc, giường cần đảm bảo vị trí ngồi, đứng thoải mái, không gây căng thẳng cho cơ bắp và xương.</li><li>Độ bền và khả năng chống mài mòn: Đồ nội thất thường phải chịu mài mòn hàng ngày từ việc sử dụng. Vì vậy, độ bền và khả năng chống mài mòn của chất liệu và cấu trúc sản phẩm là yếu tố quan trọng.</li><li>An toàn: Đồ nội thất không chỉ cần đẹp và thoải mái mà còn phải an toàn. Các cạnh sắc, phụ kiện dễ rơi rớt có thể gây nguy hiểm cho trẻ nhỏ hoặc thậm chí cả người lớn.</li></ol><p>III. Xu hướng thiết kế nội thất đương đại</p><ol><li>Phong cách tối giản: Phong cách này tập trung vào sự đơn giản, với màu sắc trung tính, đường nét tối giản và không gian rộng mở.</li><li>Tích hợp công nghệ: Với sự phát triển của công nghệ, việc tích hợp các thiết bị thông minh vào nội thất là xu hướng hiện đang rất thịnh hành.</li><li>Tái chế và bền vững: Vấn đề bền vững ngày càng được quan tâm, do đó, nội thất tái chế từ các vật liệu thân thiện với môi trường đang dần trở nên phổ biến.</li><li>Kết hợp các vật liệu truyền thống và hiện đại: Việc kết hợp các vật liệu truyền thống với hiện đại, chẳng hạn như gỗ và kim loại, tạo ra sự hài hòa độc đáo.</li></ol><p>IV. Kết luận</p><p>Trong việc chọn và thiết kế đồ nội thất, không chỉ cần quan tâm đến vẻ đẹp mà còn phải xem xét về chất lượng, tính thẩm mỹ, và tính năng sử dụng. Đồng thời, việc tuân thủ các tiêu chuẩn về an toàn và bền vững cũng rất quan trọng. Với sự phát triển không ngừng của xu hướng thiết kế và công nghệ, việc lựa chọn đồ nội thất phù hợp không chỉ tạo nên một không gian sống tốt hơn mà còn thể hiện phong cách và tâm hồn của chủ nhân.</p>",
                    Thumbnail = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/articles%2F85a8c5f4-7296-4225-bba4-32087b9034a0?alt=media&token=b73a4601-28f1-4c2a-928e-1c3713ea4438",
                    IsPublish = true,
                    Slug = "tieu-chuan-chat-luong-trong-thiet-ke-va-lua-chon-do-noi-that"},
                    new Article { UserId = user.UserId,
                    Title = "Tạo Không Gian Sống Hiện Đại với Đồ Nội Thất Đa Dạng",
                    Content = "<p>Khi nói đến việc trang trí và bố trí nội thất, không chỉ đơn thuần là việc đặt các món đồ vào trong căn phòng. Đồ nội thất chính là những nguyên liệu chủ chốt để bạn sáng tạo nên một không gian sống đẹp mắt, hiện đại và thú vị. Trong bài viết này, chúng ta sẽ khám phá những cách để tận dụng đồ nội thất một cách sáng tạo để tạo ra không gian sống độc đáo.</p><p>I. Tận dụng không gian thông minh</p><ol><li>Giường lưu trữ: Đối với các căn hộ có diện tích nhỏ, giường lưu trữ là một ý tưởng tốt để tiết kiệm không gian. Những chiếc giường này có thể được nâng lên để tạo ra không gian lưu trữ phía dưới, hoặc có thể có ngăn kéo để đựng đồ.</li><li>Bàn ăn đa năng: Chọn bàn ăn có thể mở rộng khi cần thiết để tiết kiệm diện tích trong các căn phòng nhỏ hẹp.</li><li>Kệ đa năng: Sử dụng kệ có nhiều tầng để tận dụng tường trống, tạo ra không gian lưu trữ cho sách, đồ trang sức, cây cảnh, và nhiều món đồ khác.</li></ol><p>II. Kết hợp giữa thẩm mỹ và chức năng</p><ol><li>Ghế và bàn làm việc ergonomics: Khi làm việc tại nhà trở nên phổ biến hơn, việc chọn ghế và bàn làm việc tạo sự thoải mái và hỗ trợ cho cơ thể là điều quan trọng. Ghế có đệm êm ái và có thể điều chỉnh chiều cao, cùng với bàn có độ cao phù hợp để bảo vệ sức khỏe cột sống.</li><li>Ghế và sofa thoải mái: Đồ nội thất chất lượng không chỉ đẹp mắt mà còn cần phải mang lại sự thoải mái thực sự. Ghế và sofa cần có độ đàn hồi và đệm êm ái để người dùng có thể thư giãn một cách thoải mái.</li></ol><p>III. Màu sắc và chất liệu</p><ol><li>Tương phản màu sắc: Sử dụng tương phản màu sắc giữa đồ nội thất và bức tường để tạo điểm nhấn và sự thú vị. Màu sắc tương phản có thể làm tôn lên các chi tiết đẹp của món đồ.</li><li>Sử dụng gương: Gương không chỉ là vật trang trí mà còn có khả năng tạo ra ảo giác tăng không gian. Đặt gương ở các vị trí chiến lược để tạo cảm giác không gian mở rộ.</li></ol><p>IV. Tự tạo nên không gian sống riêng biệt</p><ol><li>Tập trung vào những món đồ yêu thích: Không cần phải lấp đầy căn phòng với đồ nội thất. Chọn những món đồ yêu thích và đặc biệt, từ đó tạo nên một không gian có sự cá nhân hóa rõ nét.</li><li>Tự làm đồ handmade: Nếu bạn có khả năng thủ công, hãy thử tạo ra những món đồ handmade như gấu bông, tranh treo tường, hoặc gương trang trí. Điều này không chỉ tạo ra sự độc đáo mà còn thể hiện sự sáng tạo và tình cảm của bạn.</li></ol><p>V. Kết luận</p><p>Đồ nội thất không chỉ đơn thuần là các món đồ để đặt trong căn phòng, mà chúng có thể trở thành nguồn cảm hứng để tạo nên một không gian sống thú vị và độc đáo. Bằng cách kết hợp giữa thẩm mỹ và chức năng, sáng tạo trong việc tận dụng không gian thông minh, và tự tạo nên những món đồ cá nhân, bạn có thể tạo nên không gian sống hiện đại và độc đáo phản ánh phong cách và cá tính của chính bạn.</p>",
                    Thumbnail = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/articles%2F690dbed5-6e36-405a-93b2-c1df0d2ba85e?alt=media&token=10f06ad2-e02e-4e74-98bd-8703a09344e6",
                    IsPublish = true,
                    Slug = "tao-khong-gian-song-hien-dai-voi-do-noi-that-da-dang"},
                    new Article { UserId = user.UserId,
                    Title = "Nội thất tái chế và bền vững",
                    Content = "<ol><li>Tầm quan trọng của nội thất tái chế: Xu hướng bền vững đang ngày càng trở nên quan trọng hơn trong thiết kế nội thất. Sử dụng đồ nội thất tái chế không chỉ giúp giảm tác động môi trường mà còn tạo nên một phong cách độc đáo và thú vị cho không gian sống.</li><li>Sử dụng vật liệu tái chế: Các vật liệu tái chế như gỗ tái chế, kim loại tái chế và vải từ sợi tái chế đang được sử dụng để tạo ra các món đồ nội thất vừa thân thiện với môi trường, vừa đẹp và bền.</li><li>Đồ nội thất có tuổi thọ cao: Việc chọn các món đồ nội thất có tuổi thọ cao sẽ giảm thiểu việc thay thế thường xuyên, đóng góp vào việc giảm lượng rác thải và tài nguyên tiêu thụ.</li></ol><p>VII. Cập nhật theo xu hướng mới</p><ol><li>Xu hướng màu sắc và chất liệu: Thế giới thiết kế nội thất luôn thay đổi với thời gian, vì vậy hãy cập nhật các xu hướng màu sắc và chất liệu mới nhất. Hiện nay, màu sắc tươi sáng như màu xanh lá cây, cam đang trở nên phổ biến, cùng với việc sử dụng các chất liệu tự nhiên như gỗ và da.</li><li>Thiết kế không gian mở: Xu hướng thiết kế không gian mở vẫn đang được ưa chuộng. Việc tạo ra những kết nối giữa các không gian như phòng khách, bếp và phòng ăn giúp tạo ra sự thoải mái và tận dụng tối đa diện tích.</li><li>Phong cách retro và vintage: Một số người đang chọn lựa quay lại với phong cách retro và vintage trong thiết kế nội thất. Những món đồ có thiết kế hoài cổ như đèn trần pha lê, ghế bọc da vintage mang lại một vẻ đẹp độc đáo và lịch lãm cho không gian sống.</li></ol><p>VIII. Đồ nội thất đa chức năng</p><ol><li>Sofa trở thành giường: Những chiếc sofa có thể biến thành giường thường được ứng dụng trong các căn hộ nhỏ hoặc phòng khách nhỏ.</li><li>Bàn làm việc đa năng: Một chiếc bàn có thể mở rộng để trở thành bàn ăn hoặc khu làm việc linh hoạt.</li><li>Ngăn chứa trong giường: Các mẫu giường có ngăn chứa bên trong là giải pháp tốt để tận dụng không gian dưới giường.</li></ol><p>IX. Đồ nội thất trong không gian ngoại trời</p><ol><li>Ghế và bàn ngoài trời: Không gian ngoại trời cũng quan trọng không kém trong việc trang trí nội thất. Các bộ ghế và bàn ngoài trời với chất liệu chống thời tiết giúp bạn tận hưởng thời gian ngoại trời một cách thoải mái và thú vị.</li><li>Ghế xếp gọn: Để tiết kiệm diện tích và thuận tiện di chuyển, ghế xếp gọn là một lựa chọn tốt cho không gian ngoại trời.</li></ol><p>X. Kết luận</p><p>Tận dụng đồ nội thất một cách sáng tạo và thông minh có thể tạo ra không gian sống độc đáo, hiện đại và thú vị. Việc kết hợp giữa thẩm mỹ, chức năng và tư duy bền vững sẽ giúp bạn tạo ra một không gian sống phản ánh cá tính và phong cách của mình. Hãy thử áp dụng những ý tưởng trên để biến căn phòng của bạn thành một nơi đẹp và độc đáo.</p>",
                    Thumbnail = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/articles%2Fda2269ae-4fdc-413b-8b32-021a7672380b?alt=media&token=ae21b1f9-1176-43ed-910a-d97af310a22b",
                    IsPublish = true,
                    Slug = "noi-that-tai-che-va-ben-vung"},
                    new Article { UserId = user.UserId,
                    Title = "Đồ nội thất và tạo hình không gian nghệ thuật",
                    Content = "<ol><li>Nghệ thuật trang trí tường: Sử dụng tranh, tấm hình, hay gương trang trí để tạo điểm nhấn nghệ thuật trên tường. Việc lựa chọn các tác phẩm nghệ thuật phù hợp với phong cách và sở thích của bạn có thể làm cho căn phòng trở nên sinh động và sâu sắc hơn.</li><li>Sắp xếp đồ theo cách nghệ thuật: Thay vì đặt các món đồ một cách ngẫu nhiên, bạn có thể sắp xếp chúng theo cách tạo nên sự cân đối, động và hài hòa. Chơi với các mức độ cao thấp, nhóm các vật phẩm có màu sắc tương tự hoặc hình dáng khác nhau để tạo nên một bức tranh nội thất độc đáo.</li></ol><p>XII. Khám phá phong cách nội thất đa dạng</p><ol><li>Phong cách hiện đại và tối giản: Sự tối giản và sự hiện đại tiếp tục được yêu thích với sự tập trung vào đường nét sáng sủa và màu sắc trung tính. Bỏ đi những chi tiết không cần thiết và giữ lại những món đồ có thiết kế tối giản sẽ tạo ra không gian tràn đầy sự sang trọng.</li><li>Phong cách nội thất Bohemian: Đặc trưng bởi sự sáng tạo, màu sắc tươi sáng và việc kết hợp đa dạng về hình dáng và chất liệu, phong cách Bohemian mang đến vẻ đẹp tự do và cá tính. Sử dụng các vật trang trí thủ công, tấm thảm màu sắc và đèn trang trí có thiết kế độc đáo để tạo nên không gian nghệ thuật và thú vị.</li><li>Phong cách Scandinavian: Phong cách này tập trung vào ánh sáng tự nhiên, màu sắc trung tính, và sự thoải mái. Sử dụng gỗ tự nhiên và các món đồ có thiết kế đơn giản để tạo nên không gian ấm cúng và hiện đại.</li></ol><p>XIII. Tận dụng không gian phụ trong nhà</p><ol><li>Góc đọc sách và thư giãn: Tận dụng các góc trống trong nhà để tạo ra một góc đọc sách thoải mái với ghế êm ái và đèn đọc.</li><li>Khu vực làm việc nhỏ gọn: Nếu bạn cần một không gian làm việc nhỏ tại nhà, hãy tìm một góc trống hoặc một phần tường để đặt bàn làm việc nhỏ và các dụng cụ làm việc cần thiết.</li></ol><p>XIV. Đồ nội thất và cảm nhận giữa thiết kế và cảm xúc</p><ol><li>Tạo không gian thư giãn: Sử dụng đèn mờ, tấm thảm êm ái và các món đồ nội thất tạo cảm giác thư thái để giúp bạn thư giãn sau những ngày làm việc căng thẳng.</li><li>Tạo không gian truyền cảm hứng: Sử dụng màu sắc tươi sáng và các vật trang trí độc đáo để tạo ra một không gian đầy năng lượng và cảm hứng cho công việc sáng tạo và tư duy.</li></ol><p>XV. Nội thất và sự kết hợp âm nhạc</p><ol><li>Khu vực thư giãn với âm nhạc: Đặt một bộ loa không dây trong không gian thư giãn để bạn có thể thư thái với âm nhạc yêu thích.</li><li>Kết hợp thiết kế và âm nhạc: Sử dụng tấm vách âm thanh hoặc các bức tranh âm nhạc để kết hợp giữa nghệ thuật âm thanh và thiết kế nội thất.</li></ol><p>XVI. Đồ nội thất và sự kết nối gia đình</p><ol><li>Khu vực chơi trẻ em: Tạo một góc chơi cho trẻ em bằng cách sử dụng thảm mềm và các đồ chơi. Điều này sẽ tạo ra một không gian kết nối gia đình và thú vị.</li><li>Khu vực gia đình: Đặt một bộ sofa lớn hoặc một bàn ăn đa năng để tạo nên không gian thân thiện và thoải mái cho cả gia đình.</li></ol><p>XVII. Kết luận</p><p>Đồ nội thất không chỉ đơn thuần là những món đồ để sắp xếp, mà chúng còn có thể tạo nên những không gian sống độc đáo, sáng tạo và ấn tượng. Tận dụng sự đa dạng của đồ nội thất, bạn có thể tạo ra không gian sống phản ánh cá tính, phong cách và cảm xúc của chính bạn.</p>",
                    Thumbnail = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/articles%2Fd389d56c-9518-4d5d-975b-71a941494944?alt=media&token=d7ca20b1-9bd9-4058-8999-5d96584869c7",
                    IsPublish = true,
                    Slug = "do-noi-that-va-tao-hinh-khong-gian-nghe-thuat"},
                    new Article { UserId = user.UserId,
                    Title = "Tận dụng thiên nhiên trong nội thất",
                    Content = "<ol><li>Các yếu tố thiên nhiên: Sử dụng cây cảnh, hoa và các vật liệu tự nhiên như gỗ, đá để tạo nên không gian sống gần gũi với thiên nhiên. Các cây cảnh không chỉ làm cho không gian thêm xanh mà còn cải thiện chất lượng không khí.</li><li>Ánh sáng tự nhiên: Tận dụng ánh sáng tự nhiên bằng cách mở rộ cửa sổ, sử dụng rèm cửa mỏng để ánh sáng có thể xâm nhập sâu vào trong căn phòng. Ánh sáng tự nhiên không chỉ làm cho không gian sáng hơn mà còn có lợi cho tâm trạng và sức khỏe của bạn.</li></ol><p>XIX. Sự kết hợp giữa cổ điển và hiện đại</p><ol><li>Tạo điểm nhấn cổ điển: Thêm một món đồ cổ điển như một chiếc ghế bọc da hoặc một chiếc gương hoành tráng vào không gian hiện đại để tạo sự đối lập thú vị.</li><li>Kết hợp màu sắc và chất liệu: Kết hợp giữa màu sắc và chất liệu cổ điển với thiết kế hiện đại để tạo ra một không gian độc đáo và đầy tinh tế.</li></ol><p>XX. Tạo không gian sáng tạo và làm việc</p><ol><li>Góc làm việc sáng tạo: Tạo ra một góc làm việc sáng tạo với bàn làm việc và ghế thoải mái, cùng với các vật phẩm trang trí và nguồn ánh sáng sáng tạo.</li><li>Bảng tường ý tưởng: Sử dụng bảng tường để viết ý tưởng, ghi chú và lời nhắc, giúp bạn duy trì sự sáng tạo trong công việc và học tập.</li></ol><p>XXI. Tạo không gian thư giãn và tĩnh lặng</p><ol><li>Góc đọc sách yên tĩnh: Tạo một không gian yên tĩnh để đọc sách và thư giãn bằng cách sử dụng ghế êm ái, ánh sáng dịu và các vật phẩm tạo cảm giác thư thái.</li><li>Phòng tĩnh lặng: Nếu có điều kiện, bạn có thể dành một góc riêng để tạo phòng tĩnh lặng, nơi bạn có thể tập trung vào việc tư duy sáng tạo hoặc thiền định.</li></ol><p>XXII. Đồ nội thất và kỷ niệm gia đình</p><ol><li>Tủ sách gia đình: Tạo một tủ sách gia đình để chứa các cuốn sách yêu thích của mỗi người trong gia đình, tạo nên không gian kỷ niệm và chia sẻ.</li><li>Góc hiển thị kỷ niệm: Tạo một góc trưng bày để hiển thị các vật phẩm và ảnh kỷ niệm gia đình, tạo nên một góc nhỏ để nhớ về những khoảnh khắc đáng nhớ.</li></ol><p>XXIII. Đồ nội thất và cảm xúc nghệ thuật</p><ol><li>Màu sắc và tâm trạng: Sử dụng màu sắc tương thích với tâm trạng của bạn. Màu xanh dương thường tạo cảm giác bình yên, trong khi màu vàng sáng có thể thúc đẩy sự hạnh phúc.</li><li>Vật phẩm có ý nghĩa: Sử dụng các vật phẩm mang ý nghĩa như tranh, hình ảnh gia đình hoặc những món đồ bạn thích để tạo ra một không gian thể hiện cảm xúc nghệ thuật.</li></ol><p>XXIV. Tạo không gian chia sẻ và tiếp khách</p><ol><li>Góc tiếp khách: Tạo một góc tiếp khách ấn tượng bằng cách sử dụng ghế thoải mái, bàn trà và vật trang trí tạo điểm nhấn.</li><li>Bàn ăn lớn: Chọn một bàn ăn lớn để tạo không gian chia sẻ bữa ăn vui vẻ và ấm áp cùng gia đình và bạn bè.</li></ol><p>XXV. Kết luận</p><p>Đồ nội thất không chỉ là những món đồ để trang trí không gian sống, mà chúng còn mang trong mình sự sáng tạo, tình cảm và cảm xúc nghệ thuật. Tận dụng sự đa dạng của đồ nội thất để tạo ra không gian sống độc đáo, phản ánh cá tính và phong cách của bạn, và đặc biệt là tạo nên những khoảnh khắc đáng nhớ trong cuộc sống hàng ngày.</p>",
                    Thumbnail = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/articles%2F262f33b0-5a19-4b87-8088-c1db035d9c6b?alt=media&token=4a1eb72b-0916-4641-932a-580e3de78e82",
                    IsPublish = true,
                    Slug = "tan-dung-thien-nhien-trong-noi-that"},
                    new Article { UserId = user.UserId,
                    Title = "Khám phá Cách Tối Ưu Hóa Nội Thất để Tạo Không Gian Sống Hiện Đại và Tiện Nghi.",
                    Content = "<ol><li><strong>Trong thế giới của thiết kế nội thất</strong>, sự tối ưu hóa đang trở thành xu hướng quan trọng để tạo ra những không gian sống hiện đại và tiện nghi. Không chỉ là về việc sắp xếp đồ đạc một cách thông minh, mà còn về việc tận dụng mọi góc nhỏ để tạo nên môi trường sống thú vị và đa dạng.</li><li><strong>Khi bước vào quá trình tối ưu hóa nội thấ</strong>t, việc lựa chọn đồ nội thất phù hợp với không gian là bước đầu tiên quan trọng. Đối với căn hộ có diện tích nhỏ, nên chọn những món đồ đa năng như giường có ngăn chứa đồ, bàn trà có ngăn để sách và đồ nhỏ. Việc này giúp tận dụng mọi không gian một cách thông minh, giúp căn phòng trông rộng rãi hơn.</li><li><strong>Không gian mở</strong> cũng là một yếu tố quan trọng trong việc tối ưu hóa nội thất. Bằng cách loại bỏ các vật dụng không cần thiết và tạo ra một sự thông thoáng cho căn phòng, bạn có thể tạo nên không gian sống sạch sẽ và thoải mái. Sử dụng màu sắc nhạt và thiết kế đơn giản cũng có thể làm tăng cường cảm giác rộng rãi cho không gian.</li><li><strong>Ngoài ra,</strong> ánh sáng cũng đóng vai trò quan trọng trong việc tạo không gian sống tối ưu. Sử dụng ánh sáng tự nhiên một cách thông thoáng để thúc đẩy sự tươi mới và sinh động cho căn phòng. Bạn cũng nên cân nhắc sử dụng đèn chiếu sáng phù hợp để tạo ra các điểm nhấn ánh sáng trong không gian.</li><li>Không gian sống hiện đại cũng cần phải tối ưu hóa về khả năng sắp xếp. Lựa chọn nội thất có tính năng gấp gọn hoặc có thể thay đổi hình dáng linh hoạt giúp bạn tận dụng không gian theo nhu cầu cụ thể. Điều này đặc biệt quan trọng đối với những người sống trong các căn hộ nhỏ hẹp.</li></ol><p><strong>Tổng quan</strong>, việc tối ưu hóa nội thất là một quá trình tinh tế, yêu cầu sự cân nhắc và khả năng sáng tạo. Bằng cách lựa chọn đồ nội thất thông minh, tạo không gian mở và tối ưu hóa việc sắp xếp, bạn có thể tạo ra những không gian sống hiện đại, thoải mái và tiện nghi, phản ánh phong cách và cá nhân của bạn một cách hoàn hảo.</p>",
                    Thumbnail = "https://firebasestorage.googleapis.com/v0/b/tkdecor-acd26.appspot.com/o/articles%2Fa6bc02dd-40f2-40fa-b4e7-3317cfc4c932?alt=media&token=df26f103-cbcc-4f00-a1a2-16556c1e7838",
                    IsPublish = true,
                    Slug = "kham-pha-cach-toi-uu-hoa-noi-that-de-tao-khong-gian-song-hien-dai-va-tien-nghi"}
                };
                _db.Articles.AddRange(list);
                _db.SaveChanges();
            }
        }

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
            userAddressSetDefaults.RuleFor(x => x.Street, f => "324 hẻm 6 ");

            // Lấy danh sách các giá trị enum của OrderStatus
            List<string> genderValues = new()
            {
                SD.GenderMale,
                SD.GenderFemale,
                SD.GenderOther,
            };

            // add customer and seller
            //Role? roleCustomer = await _db.Roles.FirstOrDefaultAsync(r => r.Name == Role.Customer);
            for (int i = 0; i < 10; i++)
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
                phoneNumber += random.Next(1, 10);
            }

            return phoneNumber;
        }
    }
}
