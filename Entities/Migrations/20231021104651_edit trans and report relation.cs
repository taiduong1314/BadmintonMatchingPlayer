using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class edittransandreportrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_HistoryTransaction",
                table: "Report");

            migrationBuilder.RenameColumn(
                name: "idHistory",
                table: "Report",
                newName: "IdTransaction");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeadLine",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Reports",
                table: "Report",
                column: "IdTransaction",
                principalTable: "Transactions",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Reports",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "DeadLine",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "IdTransaction",
                table: "Report",
                newName: "idHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_HistoryTransaction",
                table: "Report",
                column: "idHistory",
                principalTable: "HistoryTransaction",
                principalColumn: "id");
        }
    }
}
