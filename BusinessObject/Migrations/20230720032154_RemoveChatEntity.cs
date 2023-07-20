using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChatEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    chat_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    receiver_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sender_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Chat__0BBF6EE6112EA443", x => x.chat_id);
                    table.ForeignKey(
                        name: "FK_Chat_User",
                        column: x => x.sender_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Chat_User1",
                        column: x => x.receiver_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_receiver_id",
                table: "Chat",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_sender_id",
                table: "Chat",
                column: "sender_id");
        }
    }
}
