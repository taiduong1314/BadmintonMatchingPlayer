using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class addpostidtoreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdPost",
                table: "Report",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Reports",
                table: "Report",
                column: "IdPost",
                principalTable: "Posts",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Reports",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "IdPost",
                table: "Report");
        }
    }
}
