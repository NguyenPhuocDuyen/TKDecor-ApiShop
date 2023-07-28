using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChatModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "ChatRoom");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRoom",
                columns: table => new
                {
                    chat_room_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    staff_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_close = table.Column<bool>(type: "bit", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
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
                name: "ChatMessage",
                columns: table => new
                {
                    chat_message_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    chat_room_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sender_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
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
        }
    }
}
