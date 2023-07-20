using Bogus;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Utility;
using Utility.SD;

namespace DataAccess.Data
{
    public class DbInitializer : IDbInitializer
    {
        private Random random = new();

        private readonly TkdecorContext _db = new();

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

            await AddUser();
            await AddArticles();
            AddCategories();
            await AddCoupons();
            await AddNotifications();
            await AddProducts();
            await AddProductImages();
            await AddCarts();
            await AddProductFavorites();
            await AddProductReviewInteractions();
            await AddProductReports();
            await AddOrders();
            await AddProductReview();
            await AddReportProductReview();
        }

        private async Task AddProductImages()
        {
            if (_db.ProductImages.Any()) return;
            var products = await _db.Products.ToListAsync();
            foreach (var product in products)
            {
                ProductImage productImage = new()
                {
                    ProductId = product.ProductId,
                    Product = product,
                    ImageUrl = "https://img.freepik.com/premium-photo/luxury-purple-color-cylinder-pedestal-podium-product-presentation-3d-rendering_41470-4246.jpg"
                };
                ProductImage productImage2 = new()
                {
                    ProductId = product.ProductId,
                    Product = product,
                    ImageUrl = "https://media.formula1.com/image/upload/f_auto/q_auto/v1679313708/fom-website/2018-redesign-assets/Tag%20collections/Other/NetZero_Car_i4_i3_0004.jpg.transform/9col/image.jpg"
                };
                _db.ProductImages.Add(productImage);
                _db.ProductImages.Add(productImage2);
            }
            _db.SaveChanges();
        }

        private async Task AddReportProductReview()
        {
            if (_db.ReportProductReviews.Any()) return;

            //get one user
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Role != Role.Admin);

            var reviews = await _db.ProductReviews
                .Include(x => x.Product)
                .ToListAsync();

            Array reportStatusValues = Enum.GetValues(typeof(ReportStatus));

            if (user != null)
            {
                foreach (var review in reviews)
                {
                    ReportStatus randomStatus = (ReportStatus)reportStatusValues.GetValue(random.Next(reportStatusValues.Length));

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

            var productReviewSetDefaults = new Faker<ProductReview>();
            productReviewSetDefaults.RuleFor(x => x.Description, f => f.Lorem.Paragraph());

            foreach (var od in orderDetail)
            {
                ProductReview productReview = productReviewSetDefaults.Generate();
                productReview.UserId = od.Order.UserId;
                productReview.User = od.Order.User;
                productReview.ProductId = od.Product.ProductId;
                productReview.Product = od.Product;
                productReview.Rate = random.Next(2, 6);
                _db.ProductReviews.Add(productReview);
            }
            _db.SaveChanges();
        }

        private async Task AddProductReports()
        {
            if (_db.ProductReports.Any()) return;


            var productReportSetDefaults = new Faker<ProductReport>();
            productReportSetDefaults.RuleFor(x => x.Reason, f => f.Lorem.Paragraph());

            var users = await _db.Users
                .Where(x => x.Role == Role.Customer)
                .ToListAsync();
            var products = await _db.Products.ToListAsync();

            Array reportStatusValues = Enum.GetValues(typeof(ReportStatus));

            foreach (var u in users)
            {
                foreach (var p in products)
                {
                    ReportStatus randomStatus = (ReportStatus)reportStatusValues.GetValue(random.Next(reportStatusValues.Length));

                    ProductReport productReport = productReportSetDefaults.Generate();
                    productReport.UserReportId = u.UserId;
                    productReport.UserReport = u;
                    productReport.ProductReportedId = p.ProductId;
                    productReport.ProductReported = p;
                    productReport.ReportStatus = randomStatus;
                    productReport.Reason = $"{u.FullName} want to report {p.Name} because {productReport.Reason}";
                    _db.ProductReports.Add(productReport);
                }
                _db.SaveChanges();
            }
        }

        private async Task AddProductReviewInteractions()
        {
            if (_db.ProductReviewInteractions.Any()) return;
            var users = await _db.Users
            .Where(x => x.Role == Role.Customer)
            .ToListAsync();

            var prouductReviews = await _db.ProductReviews.ToListAsync();
            foreach (var u in users)
            {
                foreach (var pr in prouductReviews)
                {
                    ProductReviewInteraction productInteraction = new()
                    {
                        UserId = u.UserId,
                        User = u,
                        ProductReviewId = pr.ProductId,
                        ProductReview = pr,
                        Interaction = Interaction.Like,
                    };
                    _db.ProductReviewInteractions.Add(productInteraction);
                }
            }
            _db.SaveChanges();

        }

        private async Task AddProductFavorites()
        {
            if (_db.ProductFavorites.Any()) return;

            var users = await _db.Users
                .Where(x => x.Role == Role.Customer)
                .ToListAsync();
            var products = await _db.Products.ToListAsync();
            foreach (var u in users)
            {
                foreach (var p in products)
                {
                    ProductFavorite productFavorite = new()
                    {
                        ProductId = p.ProductId,
                        Product = p,
                        UserId = u.UserId,
                        User = u
                    };
                    _db.ProductFavorites.Add(productFavorite);
                }
            }
            _db.SaveChanges();
        }

        private async Task AddProducts()
        {
            if (_db.Products.Any()) return;

            var productSetDefaults = new Faker<Product>();
            productSetDefaults.RuleFor(x => x.Name, f => f.Lorem.Word());
            productSetDefaults.RuleFor(x => x.Description, f => f.Lorem.Paragraphs());
            productSetDefaults.RuleFor(x => x.Quantity, f => f.Random.Int(0, 100));
            productSetDefaults.RuleFor(x => x.Price, f => f.Random.Int(10, 100));

            var categories = await _db.Categories.ToListAsync();
            foreach (var category in categories)
            {
                for (int i = 0; i < 2; i++)
                {
                    Product product = productSetDefaults.Generate();
                    product.CategoryId = category.CategoryId;
                    product.Category = category;
                    product.Price *= 1000;
                    //3d model...
                    Product3DModel product3DModel = new()
                    {
                        ModelName = "Model 3d: " + product.Name,
                        VideoUrl = "",
                        ModelUrl = "https://cdn-luma.com/e13d5b281b9c97e6fbc3defda9a2812bcca09f323705dff6b77646f0d5655dc9.glb",
                        ThumbnailUrl = "https://fronty.com/static/uploads/01.22-02.22/pexels-uzunov-rostislav-5011647.jpg"
                    };
                    product.Product3DModel = product3DModel;
                    product3DModel.Product = product;

                    product.Name = $"Product {i} {product.Name} of {category.Name}";
                    product.Slug = Slug.GenerateSlug(product.Name);
                    _db.Products.Add(product);
                }
            }
            _db.SaveChanges();
        }

        private async Task AddOrders()
        {
            if (_db.Orders.Any()) return;

            var users = await _db.Users
                .Include(x => x.UserAddresses.Where(ud => ud.IsDefault == true))
                .Where(x => x.Role == Role.Customer)
                .ToListAsync();
            var products = await _db.Products.Take(4).ToListAsync();

            // Lấy danh sách các giá trị enum của OrderStatus
            Array orderStatusValues = Enum.GetValues(typeof(OrderStatus));

            foreach (var u in users)
            {
                // Random một giá trị từ order status
                OrderStatus randomStatus = (OrderStatus)orderStatusValues.GetValue(random.Next(orderStatusValues.Length));
                Order order = new()
                {
                    UserId = u.UserId,
                    User = u,
                    OrderStatus = randomStatus,
                    FullName = u.UserAddresses?.FirstOrDefault()?.FullName ?? "",
                    Phone = u.UserAddresses?.FirstOrDefault()?.Phone ?? "",
                    Address = u.UserAddresses?.FirstOrDefault()?.Address ?? "",
                    Note = "Note ne",
                    TotalPrice = 0,
                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var p in products)
                {
                    int randomQuantity = random.Next(1, 5);

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

            var notificationSetDefaults = new Faker<Notification>();
            notificationSetDefaults.RuleFor(x => x.Message, f => f.Lorem.Sentence());

            var users = await _db.Users
                .Where(x => x.Role == Role.Customer)
                .ToListAsync();
            foreach (var u in users)
            {
                for (int i = 0; i < 10; i++)
                {
                    Notification notification = notificationSetDefaults.Generate();
                    notification.User = u;
                    _db.Notifications.Add(notification);
                }
            }
            _db.SaveChanges();
        }

        private async Task AddCoupons()
        {
            if (_db.Coupons.Any()) return;

            var couponSetDefault = new Faker<Coupon>();
            couponSetDefault.RuleFor(x => x.IsActive, true);
            couponSetDefault.RuleFor(x => x.RemainingUsageCount, 10);

            for (int i = 1; i < 6; i++)
            {
                Coupon coupon = couponSetDefault.Generate();
                coupon.CouponType = CouponType.ByPercent;
                coupon.Code = "CodePercent" + i;
                coupon.Value = i * 10;
                coupon.MaxValue = 100000;
                _db.Coupons.Add(coupon);
            }

            for (int i = 1; i < 6; i++)
            {
                Coupon coupon = couponSetDefault.Generate();
                coupon.CouponType = CouponType.ByValue;
                coupon.Code = "CodeValue" + i;
                coupon.Value = i * 10000;
                coupon.MaxValue = coupon.Value;
                _db.Coupons.Add(coupon);
            }
            _db.SaveChanges();
        }

        private async Task AddCarts()
        {
            if (_db.Carts.Any()) return;

            var users = await _db.Users
                .Where(x => x.Role == Role.Customer)
                .ToListAsync();

            var products = await _db.Products.ToListAsync();
            foreach (var u in users)
            {
                foreach (var p in products)
                {
                    Cart cart = new()
                    {
                        User = u,
                        Product = p,
                        Quantity = random.Next(0, 10)
                    };
                    _db.Carts.Add(cart);
                }
            }
            _db.SaveChanges();
        }

        private void AddCategories()
        {
            if (_db.Categories.Any()) return;

            var categorySetDefaults = new Faker<Category>();
            categorySetDefaults.RuleFor(x => x.Name, f => f.Lorem.Word());
            categorySetDefaults.RuleFor(x => x.Thumbnail, "https://takepsd.com/wp-content/uploads/2020/11/116020272.jpg");
            for (int i = 0; i < 10; i++)
            {
                Category category = categorySetDefaults.Generate();
                category.Name = $"Category {i}: {category.Name}";
                _db.Categories.Add(category);
            }
            _db.SaveChanges();
        }

        private async Task AddArticles()
        {
            if (_db.Articles.Any()) return;

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Role == Role.Admin);
            if (user != null)
            {
                var articleSetDefaults = new Faker<Article>();
                articleSetDefaults.RuleFor(x => x.User, user);
                articleSetDefaults.RuleFor(x => x.Title, f => f.Lorem.Sentence());
                articleSetDefaults.RuleFor(x => x.Content, f => f.Lorem.Paragraphs());
                articleSetDefaults.RuleFor(x => x.Thumbnail, "https://kinsta.com/wp-content/uploads/2020/08/tiger-jpg.jpg");
                articleSetDefaults.RuleFor(x => x.IsPublish, true);
                for (var i = 0; i < 20; i++)
                {
                    Article article = articleSetDefaults.Generate();
                    article.Title = "Article 1: " + article.Title;
                    article.Slug = Slug.GenerateSlug(article.Title);
                    _db.Articles.Add(article);
                }
                _db.SaveChanges();
            }
        }

        private async Task AddUser()
        {
            if (_db.Users.Any()) return;

            // add admin
            User admin = new()
            {
                Email = "admin@gmail.com",
                Password = Password.HashPassword("admin@gmail.com"),
                FullName = "admin",
                Role = Role.Admin,
                BirthDay = DateTime.Now,
                Gender = Gender.Male,
                EmailConfirmed = true,
                AvatarUrl = "https://static.vecteezy.com/system/resources/previews/000/439/863/original/vector-users-icon.jpg",
            };
            _db.Users.Add(admin);

            var userSetDefaults = new Faker<User>();
            userSetDefaults.RuleFor(a => a.Email, "@gmail.com");
            userSetDefaults.RuleFor(a => a.Password, Password.HashPassword("default@123"));
            userSetDefaults.RuleFor(a => a.FullName, f => f.Lorem.Word());
            userSetDefaults.RuleFor(a => a.EmailConfirmed, true);
            userSetDefaults.RuleFor(a => a.AvatarUrl, "https://static.vecteezy.com/system/resources/previews/000/439/863/original/vector-users-icon.jpg");

            var userAddressSetDefaults = new Faker<UserAddress>();
            userAddressSetDefaults.RuleFor(x => x.Address, f => f.Lorem.Sentence());
            userAddressSetDefaults.RuleFor(x => x.CityCode, 1);
            userAddressSetDefaults.RuleFor(x => x.City, "Thành phố Hà Nội");
            userAddressSetDefaults.RuleFor(x => x.DistrictCode, 1);
            userAddressSetDefaults.RuleFor(x => x.District, "Quận Ba Đình");
            userAddressSetDefaults.RuleFor(x => x.WardCode, 1);
            userAddressSetDefaults.RuleFor(x => x.Ward, "Phường Phúc Xá");
            userAddressSetDefaults.RuleFor(x => x.Street, "324 hẻm 6");

            // Lấy danh sách các giá trị enum của OrderStatus
            Array genderValues = Enum.GetValues(typeof(Gender));

            // add customer and seller
            //Role? roleCustomer = await _db.Roles.FirstOrDefaultAsync(r => r.Name == Role.Customer);
            List<Role> roles = new() { Role.Seller, Role.Customer };
            foreach (var role in roles)
            {
                for (int i = 0; i < 10; i++)
                {
                    Gender randomStatus = (Gender)genderValues.GetValue(random.Next(genderValues.Length));
                    User u = userSetDefaults.Generate();
                    u.Role = role;
                    u.Email = $"{role}{i}{u.Email}";
                    u.FullName = $"{role} {i} {u.FullName}";
                    u.UserAddresses = new List<UserAddress>();
                    u.BirthDay = DateTime.Now;
                    u.Gender = randomStatus;

                    for (int j = 0; j < 4; j++)
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
