using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessObject;

public partial class TkdecorContext : DbContext
{
    public TkdecorContext()
    {
    }

    public TkdecorContext(DbContextOptions<TkdecorContext> options)
        : base(options)
    {
    }

    #region DbSet
    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<CouponType> CouponTypes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductInteraction> ProductInteractions { get; set; }

    public virtual DbSet<ProductInteractionStatus> ProductInteractionStatuses { get; set; }

    public virtual DbSet<ProductReport> ProductReports { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<ReportProductReview> ReportProductReviews { get; set; }

    public virtual DbSet<ReportStatus> ReportStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    #endregion
    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=localhost;Database=TKDecor;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(e => e.RefreshTokenId).HasColumnName("refresh_token_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.JwtId).HasColumnName("jwt_id");
            entity.Property(e => e.IsUsed).HasColumnName("is_used");
            entity.Property(e => e.IsRevoked).HasColumnName("is_revoked");
            entity.Property(e => e.IssuedAt)
                .HasColumnType("datetime")
                .HasColumnName("issued_at");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("datetime")
                .HasColumnName("expired_at");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshToken_User");

        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Article__CC36F660180F7E7F");

            entity.ToTable("Article");

            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDelete).HasColumnName("is_delete");
            entity.Property(e => e.IsPublish).HasColumnName("is_publish");
            entity.Property(e => e.Thumbnail).HasColumnName("thumbnail");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Articles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Article_User");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__2EF52A27823B5610");

            entity.ToTable("Cart");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_Product");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__D54EE9B430138B56");

            entity.ToTable("Category");

            entity.HasIndex(e => e.Name, "UQ__Category__72E12F1BE7D29FE9").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDelete).HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PK__Coupon__58CF6389A836CD8D");

            entity.ToTable("Coupon");

            entity.HasIndex(e => e.Code, "UQ__Coupon__357D4CF9C0A7C258").IsUnique();

            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CouponTypeId).HasColumnName("coupon_type_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.RemainingUsageCount).HasColumnName("remaining_usage_count");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Value)
                .HasColumnType("decimal(8, 0)")
                .HasColumnName("value");

            entity.HasOne(d => d.CouponType).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.CouponTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Coupon_CouponType");
        });

        modelBuilder.Entity<CouponType>(entity =>
        {
            entity.HasKey(e => e.CouponTypeId).HasName("PK__CouponTy__AD2AFC0A104B34A3");

            entity.ToTable("CouponType");

            entity.HasIndex(e => e.Name, "UQ__CouponTy__72E12F1BE5D8D49E").IsUnique();

            entity.Property(e => e.CouponTypeId).HasColumnName("coupon_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__0BBF6EE6112EA443");

            entity.ToTable("Message");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message1).HasColumnName("message");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_User1");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F2D0624C3");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__465962292D23A46C");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Coupon).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CouponId)
                .HasConstraintName("FK_Order_Coupon");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_OrderStatus");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__3C5A4080BBA82BF6");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentPrice)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("payment_price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Product");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PK__OrderSta__A499CF231D746F37");

            entity.ToTable("OrderStatus");

            entity.HasIndex(e => e.Name, "UQ__OrderSta__72E12F1B9EE1EE34").IsUnique();

            entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__47027DF5960BA9EF");

            entity.ToTable("Product");

            entity.HasIndex(e => e.Name, "UQ__Product__72E12F1B7465289F").IsUnique();
            entity.HasIndex(e => e.Slug, "UQ__Product__72E12F1B1235289A").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDelete).HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Url3dModel)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("url_3D_model");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ProductImageId).HasName("PK__ProductI__7A342910809D9053");

            entity.ToTable("ProductImage");

            entity.Property(e => e.ProductImageId)
                .ValueGeneratedOnAdd()
                .HasColumnName("productImage_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.IsDelete).HasColumnName("is_delete");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.ProductImageNavigation).WithOne(p => p.ProductImage)
                .HasForeignKey<ProductImage>(d => d.ProductImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImage_Product");
        });

        modelBuilder.Entity<ProductInteraction>(entity =>
        {
            entity.HasKey(e => e.ProductInteractionId).HasName("PK__ProductI__A2F12EDBBAC0359E");

            entity.ToTable("ProductInteraction");

            entity.Property(e => e.ProductInteractionId).HasColumnName("product_interaction_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductInteractionStatusId).HasColumnName("product_interaction_status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductInteractions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductInteraction_Product");

            entity.HasOne(d => d.ProductInteractionStatus).WithMany(p => p.ProductInteractions)
                .HasForeignKey(d => d.ProductInteractionStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductInteraction_ProductInteractionStatus");

            entity.HasOne(d => d.User).WithMany(p => p.ProductInteractions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductInteraction_User");
        });

        modelBuilder.Entity<ProductInteractionStatus>(entity =>
        {
            entity.HasKey(e => e.ProductInteractionStatusId).HasName("PK__ProductI__0CC0092882FD7968");

            entity.ToTable("ProductInteractionStatus");

            entity.HasIndex(e => e.Name, "UQ__ProductI__72E12F1BB1BB6955").IsUnique();

            entity.Property(e => e.ProductInteractionStatusId).HasColumnName("product_interaction_status_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProductReport>(entity =>
        {
            entity.HasKey(e => e.ProductReportId).HasName("PK__ProductR__DC0B4A22C1B82B5E");

            entity.ToTable("ProductReport");

            entity.Property(e => e.ProductReportId).HasColumnName("product_report_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductReportedId).HasColumnName("product_reported_id");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.ReportStatusId).HasColumnName("report_status_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserReportId).HasColumnName("user_report_id");

            entity.HasOne(d => d.ProductReported).WithMany(p => p.ProductReports)
                .HasForeignKey(d => d.ProductReportedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReport_Product");

            entity.HasOne(d => d.ReportStatus).WithMany(p => p.ProductReports)
                .HasForeignKey(d => d.ReportStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReport_ReportStatus");

            entity.HasOne(d => d.UserReport).WithMany(p => p.ProductReports)
                .HasForeignKey(d => d.UserReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReport_User");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ProductReviewId).HasName("PK__ProductR__8440EB03DB89B961");

            entity.ToTable("ProductReview");

            entity.Property(e => e.ProductReviewId).HasColumnName("product_review_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDelete).HasColumnName("is_delete");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReview_Product");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReview_User");
        });

        modelBuilder.Entity<ReportProductReview>(entity =>
        {
            entity.HasKey(e => e.ReportProductReviewId).HasName("PK__ReportPr__09EE80B34DFBA358");

            entity.ToTable("ReportProductReview");

            entity.Property(e => e.ReportProductReviewId).HasColumnName("report_product_review_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductReviewReportedId).HasColumnName("product_review_reported_id");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.ReportStatusId).HasColumnName("report_status_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserReportId).HasColumnName("user_report_id");

            entity.HasOne(d => d.ProductReviewReported).WithMany(p => p.ReportProductReviews)
                .HasForeignKey(d => d.ProductReviewReportedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportProductReview_ProductReview");

            entity.HasOne(d => d.ReportStatus).WithMany(p => p.ReportProductReviews)
                .HasForeignKey(d => d.ReportStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportProductReview_ReportStatus");

            entity.HasOne(d => d.UserReport).WithMany(p => p.ReportProductReviews)
                .HasForeignKey(d => d.UserReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportProductReview_User");
        });

        modelBuilder.Entity<ReportStatus>(entity =>
        {
            entity.HasKey(e => e.ReportStatusId).HasName("PK__ReportSt__09E0D88687C41A16");

            entity.ToTable("ReportStatus");

            entity.HasIndex(e => e.Name, "UQ__ReportSt__72E12F1B690C91E7").IsUnique();

            entity.Property(e => e.ReportStatusId).HasColumnName("report_status_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CC5F8795DF");

            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ__Role__72E12F1B88C9766C").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__B9BE370F0AD19949");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__AB6E6164CB4E6B2A").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EmailConfirmationCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email_confirmation_code");
            entity.Property(e => e.EmailConfirmationSentAt)
                .HasColumnType("datetime")
                .HasColumnName("email_confirmation_sent_at");
            entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsDelete).HasColumnName("is_delete");
            entity.Property(e => e.IsSubscriber).HasColumnName("is_subscriber");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.ResetPasswordCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reset_password_code");
            entity.Property(e => e.ResetPasswordRequired).HasColumnName("reset_password_required");
            entity.Property(e => e.ResetPasswordSentAt)
                .HasColumnType("datetime")
                .HasColumnName("reset_password_sent_at");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.UserAddressId).HasName("PK__UserAddr__FEC0352E4E3DC7B4");

            entity.ToTable("UserAddress");

            entity.Property(e => e.UserAddressId).HasColumnName("user_address_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAddress_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
