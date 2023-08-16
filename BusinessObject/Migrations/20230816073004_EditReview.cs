using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class EditReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReview_Product",
                table: "ProductReview");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReview_User",
                table: "ProductReview");

            migrationBuilder.DropIndex(
                name: "IX_ProductReview_product_id",
                table: "ProductReview");

            migrationBuilder.DropIndex(
                name: "IX_ProductReview_user_id",
                table: "ProductReview");

            migrationBuilder.DropColumn(
                name: "product_id",
                table: "ProductReview");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "ProductReview");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductReviewId",
                table: "OrderDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductReviewId",
                table: "OrderDetail",
                column: "ProductReviewId",
                unique: true,
                filter: "[ProductReviewId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_ProductReview_ProductReviewId",
                table: "OrderDetail",
                column: "ProductReviewId",
                principalTable: "ProductReview",
                principalColumn: "product_review_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_ProductReview_ProductReviewId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_ProductReviewId",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "ProductReviewId",
                table: "OrderDetail");

            migrationBuilder.AddColumn<Guid>(
                name: "product_id",
                table: "ProductReview",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "ProductReview",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProductReview_product_id",
                table: "ProductReview",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReview_user_id",
                table: "ProductReview",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReview_Product",
                table: "ProductReview",
                column: "product_id",
                principalTable: "Product",
                principalColumn: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReview_User",
                table: "ProductReview",
                column: "user_id",
                principalTable: "User",
                principalColumn: "user_id");
        }
    }
}
