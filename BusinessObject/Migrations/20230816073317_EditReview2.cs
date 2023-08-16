using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class EditReview2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_ProductReview_ProductReviewId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_ProductReviewId",
                table: "OrderDetail");

            migrationBuilder.RenameColumn(
                name: "ProductReviewId",
                table: "OrderDetail",
                newName: "product_review_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_product_review_id",
                table: "OrderDetail",
                column: "product_review_id",
                unique: true,
                filter: "[product_review_id] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_ProductReview_product_review_id",
                table: "OrderDetail",
                column: "product_review_id",
                principalTable: "ProductReview",
                principalColumn: "product_review_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_ProductReview_product_review_id",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_product_review_id",
                table: "OrderDetail");

            migrationBuilder.RenameColumn(
                name: "product_review_id",
                table: "OrderDetail",
                newName: "ProductReviewId");

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
    }
}
