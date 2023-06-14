﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    update_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__D54EE9B430138B56", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "CouponType",
                columns: table => new
                {
                    coupon_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CouponTy__AD2AFC0A104B34A3", x => x.coupon_type_id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    order_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderSta__A499CF231D746F37", x => x.order_status_id);
                });

            migrationBuilder.CreateTable(
                name: "Product3DModel",
                columns: table => new
                {
                    product_3d_model_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    video_url = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    model_url = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    thumbnail_url = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product3__7A982983809D9053", x => x.product_3d_model_id);
                });

            migrationBuilder.CreateTable(
                name: "ProductInteractionStatus",
                columns: table => new
                {
                    product_review_interaction_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductI__0CC0092882FD7968", x => x.product_review_interaction_status_id);
                });

            migrationBuilder.CreateTable(
                name: "ReportStatus",
                columns: table => new
                {
                    report_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportSt__09E0D88687C41A16", x => x.report_status_id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__760965CC5F8795DF", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    coupon_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    coupon_type_id = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    value = table.Column<decimal>(type: "decimal(8,0)", nullable: false),
                    remaining_usage_count = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Coupon__58CF6389A836CD8D", x => x.coupon_id);
                    table.ForeignKey(
                        name: "FK_Coupon_CouponType",
                        column: x => x.coupon_type_id,
                        principalTable: "CouponType",
                        principalColumn: "coupon_type_id");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    product_3d_model_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__47027DF5960BA9EF", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_Product_Category",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "FK_Product_Product3DModel_product_3d_model_id",
                        column: x => x.product_3d_model_id,
                        principalTable: "Product3DModel",
                        principalColumn: "product_3d_model_id");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    avatar_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    is_subscriber = table.Column<bool>(type: "bit", nullable: true),
                    email_confirmed = table.Column<bool>(type: "bit", nullable: true),
                    email_confirmation_code = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    email_confirmation_sent_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    reset_password_required = table.Column<bool>(type: "bit", nullable: true),
                    reset_password_code = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    reset_password_sent_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__B9BE370F0AD19949", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_User_Role",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    productImage_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductI__7A342910809D9053", x => x.productImage_id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    article_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_publish = table.Column<bool>(type: "bit", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Article__CC36F660180F7E7F", x => x.article_id);
                    table.ForeignKey(
                        name: "FK_Article_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart__2EF52A27823B5610", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_Cart_Product",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_Cart_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sender_id = table.Column<int>(type: "int", nullable: false),
                    receiver_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Message__0BBF6EE6112EA443", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_Message_User",
                        column: x => x.sender_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Message_User1",
                        column: x => x.receiver_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__E059842F2D0624C3", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_Notification_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    order_status_id = table.Column<int>(type: "int", nullable: false),
                    coupon_id = table.Column<int>(type: "int", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    total_price = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order__465962292D23A46C", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_Order_Coupon",
                        column: x => x.coupon_id,
                        principalTable: "Coupon",
                        principalColumn: "coupon_id");
                    table.ForeignKey(
                        name: "FK_Order_OrderStatus",
                        column: x => x.order_status_id,
                        principalTable: "OrderStatus",
                        principalColumn: "order_status_id");
                    table.ForeignKey(
                        name: "FK_Order_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ProductFavorite",
                columns: table => new
                {
                    ProductFavoriteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFavorite", x => x.ProductFavoriteId);
                    table.ForeignKey(
                        name: "FK_ProductFavorite_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFavorite_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductInteraction",
                columns: table => new
                {
                    product_review_interaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    product_interaction_status_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductI__A2F12EDBBAC0359E", x => x.product_review_interaction_id);
                    table.ForeignKey(
                        name: "FK_ProductInteraction_ProductInteractionStatus",
                        column: x => x.product_interaction_status_id,
                        principalTable: "ProductInteractionStatus",
                        principalColumn: "product_review_interaction_status_id");
                    table.ForeignKey(
                        name: "FK_ProductInteraction_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_ProductReviewInteraction_Product",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "ProductReport",
                columns: table => new
                {
                    product_report_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_report_id = table.Column<int>(type: "int", nullable: false),
                    product_reported_id = table.Column<int>(type: "int", nullable: false),
                    report_status_id = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductR__DC0B4A22C1B82B5E", x => x.product_report_id);
                    table.ForeignKey(
                        name: "FK_ProductReport_Product",
                        column: x => x.product_reported_id,
                        principalTable: "Product",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_ProductReport_ReportStatus",
                        column: x => x.report_status_id,
                        principalTable: "ReportStatus",
                        principalColumn: "report_status_id");
                    table.ForeignKey(
                        name: "FK_ProductReport_User",
                        column: x => x.user_report_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ProductReview",
                columns: table => new
                {
                    product_review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    rate = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductR__8440EB03DB89B961", x => x.product_review_id);
                    table.ForeignKey(
                        name: "FK_ProductReview_Product",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_ProductReview_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    refresh_token_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jwt_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_used = table.Column<bool>(type: "bit", nullable: false),
                    is_revoked = table.Column<bool>(type: "bit", nullable: false),
                    issued_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    expired_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.refresh_token_id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "UserAddress",
                columns: table => new
                {
                    user_address_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_default = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserAddr__FEC0352E4E3DC7B4", x => x.user_address_id);
                    table.ForeignKey(
                        name: "FK_UserAddress_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    order_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    payment_price = table.Column<decimal>(type: "decimal(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderDet__3C5A4080BBA82BF6", x => x.order_detail_id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order",
                        column: x => x.order_id,
                        principalTable: "Order",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "ReportProductReview",
                columns: table => new
                {
                    report_product_review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_report_id = table.Column<int>(type: "int", nullable: false),
                    product_review_reported_id = table.Column<int>(type: "int", nullable: false),
                    report_status_id = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportPr__09EE80B34DFBA358", x => x.report_product_review_id);
                    table.ForeignKey(
                        name: "FK_ReportProductReview_ProductReview",
                        column: x => x.product_review_reported_id,
                        principalTable: "ProductReview",
                        principalColumn: "product_review_id");
                    table.ForeignKey(
                        name: "FK_ReportProductReview_ReportStatus",
                        column: x => x.report_status_id,
                        principalTable: "ReportStatus",
                        principalColumn: "report_status_id");
                    table.ForeignKey(
                        name: "FK_ReportProductReview_User",
                        column: x => x.user_report_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_title",
                table: "Article",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Article_user_id",
                table: "Article",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_product_id",
                table: "Cart",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_user_id",
                table: "Cart",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Category_name",
                table: "Category",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_code",
                table: "Coupon",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_coupon_type_id",
                table: "Coupon",
                column: "coupon_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Message_receiver_id",
                table: "Message",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_Message_sender_id",
                table: "Message",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_user_id",
                table: "Notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_coupon_id",
                table: "Order",
                column: "coupon_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_order_status_id",
                table: "Order",
                column: "order_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_user_id",
                table: "Order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_order_id",
                table: "OrderDetail",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_product_id",
                table: "OrderDetail",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_3d_Model_id",
                table: "Product",
                column: "product_3d_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_category_id",
                table: "Product",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_product_3d_model_id",
                table: "Product",
                column: "product_3d_model_id",
                unique: true,
                filter: "[product_3d_model_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Product_slug",
                table: "Product",
                column: "slug",
                unique: true,
                filter: "([slug] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFavorite_ProductId",
                table: "ProductFavorite",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFavorite_UserId",
                table: "ProductFavorite",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_product_id",
                table: "ProductImage",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInteraction_product_id",
                table: "ProductInteraction",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInteraction_product_interaction_status_id",
                table: "ProductInteraction",
                column: "product_interaction_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInteraction_user_id",
                table: "ProductInteraction",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReport_product_reported_id",
                table: "ProductReport",
                column: "product_reported_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReport_report_status_id",
                table: "ProductReport",
                column: "report_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReport_user_report_id",
                table: "ProductReport",
                column: "user_report_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReview_product_id",
                table: "ProductReview",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReview_user_id",
                table: "ProductReview",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_user_id",
                table: "RefreshToken",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportProductReview_product_review_reported_id",
                table: "ReportProductReview",
                column: "product_review_reported_id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportProductReview_report_status_id",
                table: "ReportProductReview",
                column: "report_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportProductReview_user_report_id",
                table: "ReportProductReview",
                column: "user_report_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_role_id",
                table: "User",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_user_id",
                table: "UserAddress",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "ProductFavorite");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "ProductInteraction");

            migrationBuilder.DropTable(
                name: "ProductReport");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "ReportProductReview");

            migrationBuilder.DropTable(
                name: "UserAddress");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "ProductInteractionStatus");

            migrationBuilder.DropTable(
                name: "ProductReview");

            migrationBuilder.DropTable(
                name: "ReportStatus");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "CouponType");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Product3DModel");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
