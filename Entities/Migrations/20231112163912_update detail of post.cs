using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class updatedetailofpost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Days",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "priceSlot",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "quantitySlot",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Posts",
                newName: "SlotsInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SlotsInfo",
                table: "Posts",
                newName: "StartTime");

            migrationBuilder.AddColumn<string>(
                name: "Days",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "priceSlot",
                table: "Posts",
                type: "money",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "quantitySlot",
                table: "Posts",
                type: "int",
                nullable: true);
        }
    }
}
