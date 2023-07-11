using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class editNameTableFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFavorite_Product_ProductId",
                table: "ProductFavorite");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFavorite_User_UserId",
                table: "ProductFavorite");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProductFavorite",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductFavorite",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "product_d",
                table: "ProductFavorite",
                newName: "product_favorite_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFavorite_Product_product_id",
                table: "ProductFavorite",
                column: "product_id",
                principalTable: "Product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFavorite_User_user_id",
                table: "ProductFavorite",
                column: "user_id",
                principalTable: "User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFavorite_Product_product_id",
                table: "ProductFavorite");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFavorite_User_user_id",
                table: "ProductFavorite");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "ProductFavorite",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "ProductFavorite",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "product_favorite_id",
                table: "ProductFavorite",
                newName: "product_d");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFavorite_Product_ProductId",
                table: "ProductFavorite",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFavorite_User_UserId",
                table: "ProductFavorite",
                column: "UserId",
                principalTable: "User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
