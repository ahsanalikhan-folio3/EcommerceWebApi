using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedfieldsofCancelledOrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_ApplicationUsers_UserId",
                table: "CancelledOrders");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CancelledOrders",
                newName: "CancelledById");

            migrationBuilder.RenameIndex(
                name: "IX_CancelledOrders_UserId",
                table: "CancelledOrders",
                newName: "IX_CancelledOrders_CancelledById");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "CancelledOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_ApplicationUsers_CancelledById",
                table: "CancelledOrders",
                column: "CancelledById",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_ApplicationUsers_CancelledById",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "CancelledOrders");

            migrationBuilder.RenameColumn(
                name: "CancelledById",
                table: "CancelledOrders",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CancelledOrders_CancelledById",
                table: "CancelledOrders",
                newName: "IX_CancelledOrders_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_ApplicationUsers_UserId",
                table: "CancelledOrders",
                column: "UserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
