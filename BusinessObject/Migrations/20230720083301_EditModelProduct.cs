using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class EditModelProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_slug",
                table: "Product");

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
                        name: "FK_ChatRoom_User",
                        column: x => x.staff_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_ChatRoom_User_customer_id",
                        column: x => x.customer_id,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    ChatMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.ChatMessageId);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatRoom_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRoom",
                        principalColumn: "chat_room_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_User_SenderId",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_slug",
                table: "Product",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatRoomId",
                table: "ChatMessages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoom_customer_id",
                table: "ChatRoom",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoom_staff_id",
                table: "ChatRoom",
                column: "staff_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatRoom");

            migrationBuilder.DropIndex(
                name: "IX_Product_slug",
                table: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_Product_slug",
                table: "Product",
                column: "slug",
                unique: true,
                filter: "([slug] IS NOT NULL)");
        }
    }
}
