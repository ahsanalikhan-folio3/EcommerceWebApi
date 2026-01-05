using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedSellerIdFKinFeedbackTableFIX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_SellerProfiles_UserId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_SellerId",
                table: "Feedbacks",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_SellerProfiles_SellerId",
                table: "Feedbacks",
                column: "SellerId",
                principalTable: "SellerProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_SellerProfiles_SellerId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_SellerId",
                table: "Feedbacks");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_SellerProfiles_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "SellerProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
