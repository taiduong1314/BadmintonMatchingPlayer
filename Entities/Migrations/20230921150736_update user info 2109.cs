using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class updateuserinfo2109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayingArea",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayingLevel",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlayingWay",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SortProfile",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VerifyToken",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyToken", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tokens_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerifyToken");

            migrationBuilder.DropColumn(
                name: "PlayingArea",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PlayingLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PlayingWay",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SortProfile",
                table: "Users");
        }
    }
}
