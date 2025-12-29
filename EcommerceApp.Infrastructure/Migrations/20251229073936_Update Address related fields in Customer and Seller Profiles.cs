using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddressrelatedfieldsinCustomerandSellerProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "CustomerProfiles");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "CustomerProfiles");

            migrationBuilder.RenameColumn(
                name: "StoreAddress",
                table: "SellerProfiles",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "SellerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "SellerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "SellerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "City",
                table: "CustomerProfiles");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "CustomerProfiles");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "CustomerProfiles");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "CustomerProfiles");

            migrationBuilder.DropColumn(
                name: "State",
                table: "CustomerProfiles");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "CustomerProfiles");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "SellerProfiles",
                newName: "StoreAddress");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
