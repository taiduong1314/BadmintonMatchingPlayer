using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class addpksubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Subscription",
                type: "int",
                nullable: false,
                defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey("PK_Subcriptions", "Subscription", "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Subscription");
        }
    }
}
