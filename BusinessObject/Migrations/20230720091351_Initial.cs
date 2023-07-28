using System;
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
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__D54EE9B430138B56", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    coupon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    coupon_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    code = table.Column<string>(type: "varchar(900)", unicode: false, nullable: false),
                    value = table.Column<decimal>(type: "decimal(8,0)", nullable: false),
                    max_value = table.Column<decimal>(type: "decimal(8,0)", nullable: false),
                    remaining_usage_count = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Coupon__58CF6389A836CD8D", x => x.coupon_id);
                });

            migrationBuilder.CreateTable(
                name: "Product3DModel",
                columns: table => new
                {
                    product_3d_model_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    model_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    video_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    model_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    thumbnail_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product3__7A982983809D9053", x => x.product_3d_model_id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    password = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    birth_day = table.Column<DateTime>(type: "datetime", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    avatar_url = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    is_subscriber = table.Column<bool>(type: "bit", nullable: false),
                    email_confirmed = table.Column<bool>(type: "bit", nullable: false),
                    email_confirmation_code = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    email_confirmation_sent_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    reset_password_required = table.Column<bool>(type: "bit", nullable: false),
                    reset_password_code = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    reset_password_sent_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__B9BE370F0AD19949", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_3d_model_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Article",
                columns: table => new
                {
                    article_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    is_publish = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                name: "ChatRoom",
                columns: table => new
                {
                    chat_room_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    staff_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_close = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChatRoom__58CF6536A836CD8D", x => x.chat_room_id);
                    table.ForeignKey(
                        name: "FK_ChatRoom_User_customer_id",
                        column: x => x.customer_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_ChatRoom_User_staff_id",
                        column: x => x.staff_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    coupon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_price = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                        name: "FK_Order_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    refresh_token_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    user_address_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    city_code = table.Column<int>(type: "int", nullable: false),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    district_code = table.Column<int>(type: "int", nullable: false),
                    district = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ward_code = table.Column<int>(type: "int", nullable: false),
                    ward = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_default = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Cart",
                columns: table => new
                {
                    cart_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                name: "ProductFavorite",
                columns: table => new
                {
                    product_favorite_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFavorite", x => x.product_favorite_id);
                    table.ForeignKey(
                        name: "FK_ProductFavorite_Product_product_id",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFavorite_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    productImage_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_url = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
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
                name: "ProductReport",
                columns: table => new
                {
                    product_report_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_report_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_reported_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    report_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                        name: "FK_ProductReport_User",
                        column: x => x.user_report_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ProductReview",
                columns: table => new
                {
                    product_review_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rate = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                name: "ChatMessage",
                columns: table => new
                {
                    chat_message_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    chat_room_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sender_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChatMessage__59CF6536A836CD8D", x => x.chat_message_id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_ChatRoom_chat_room_id",
                        column: x => x.chat_room_id,
                        principalTable: "ChatRoom",
                        principalColumn: "chat_room_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessage_User_sender_id",
                        column: x => x.sender_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    order_detail_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "ProductReviewInteraction",
                columns: table => new
                {
                    product_review_interaction_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_review_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    interaction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductI__A2F12EDBBAC0359E", x => x.product_review_interaction_id);
                    table.ForeignKey(
                        name: "FK_ProductReviewInteraction_Product",
                        column: x => x.product_review_id,
                        principalTable: "ProductReview",
                        principalColumn: "product_review_id");
                    table.ForeignKey(
                        name: "FK_ProductReviewInteraction_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ReportProductReview",
                columns: table => new
                {
                    report_product_review_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_report_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_review_reported_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    report_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false)
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
                        name: "FK_ReportProductReview_User",
                        column: x => x.user_report_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_slug",
                table: "Article",
                column: "slug",
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
                name: "IX_ChatMessage_chat_room_id",
                table: "ChatMessage",
                column: "chat_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_sender_id",
                table: "ChatMessage",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoom_customer_id",
                table: "ChatRoom",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoom_staff_id",
                table: "ChatRoom",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_code",
                table: "Coupon",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_user_id",
                table: "Notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_coupon_id",
                table: "Order",
                column: "coupon_id");

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFavorite_ProductId",
                table: "ProductFavorite",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFavorite_UserId",
                table: "ProductFavorite",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_product_id",
                table: "ProductImage",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReport_product_reported_id",
                table: "ProductReport",
                column: "product_reported_id");

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
                name: "IX_ProductReviewInteraction_product_review_id",
                table: "ProductReviewInteraction",
                column: "product_review_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviewInteraction_user_id",
                table: "ProductReviewInteraction",
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
                name: "IX_ReportProductReview_user_report_id",
                table: "ReportProductReview",
                column: "user_report_id");

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
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "ProductFavorite");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "ProductReport");

            migrationBuilder.DropTable(
                name: "ProductReviewInteraction");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "ReportProductReview");

            migrationBuilder.DropTable(
                name: "UserAddress");

            migrationBuilder.DropTable(
                name: "ChatRoom");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "ProductReview");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Product3DModel");
        }
    }
}
