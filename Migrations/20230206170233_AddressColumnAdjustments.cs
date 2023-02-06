using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantAPI.Migrations
{
    public partial class AddressColumnAdjustments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Street",
                table: "Adresses");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Adresses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Adresses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Street",
                table: "Adresses");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Adresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "Street",
                table: "Adresses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
