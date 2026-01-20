using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedFKsofChatsandMessagesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_CustomerId",
                table: "Chats",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ApplicationUsers_CustomerId",
                table: "Chats",
                column: "CustomerId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ApplicationUsers_SellerId",
                table: "Chats",
                column: "SellerId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ApplicationUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ApplicationUsers_CustomerId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ApplicationUsers_SellerId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ApplicationUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_CustomerId",
                table: "Chats");
        }
    }
}
