using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class updateslotandtran : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Slot",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_idSlot",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "idSlot",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "Slot",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId1",
                table: "Slot",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "Report",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slot_TransactionId",
                table: "Slot",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Slot_TransactionId1",
                table: "Slot",
                column: "TransactionId1",
                unique: true,
                filter: "[TransactionId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Slot_Transaction",
                table: "Slot",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Slot_Transactions_TransactionId1",
                table: "Slot",
                column: "TransactionId1",
                principalTable: "Transactions",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slot_Transaction",
                table: "Slot");

            migrationBuilder.DropForeignKey(
                name: "FK_Slot_Transactions_TransactionId1",
                table: "Slot");

            migrationBuilder.DropIndex(
                name: "IX_Slot_TransactionId",
                table: "Slot");

            migrationBuilder.DropIndex(
                name: "IX_Slot_TransactionId1",
                table: "Slot");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Slot");

            migrationBuilder.DropColumn(
                name: "TransactionId1",
                table: "Slot");

            migrationBuilder.AlterColumn<bool>(
                name: "status",
                table: "Transactions",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idSlot",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "status",
                table: "Report",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_idSlot",
                table: "Transactions",
                column: "idSlot");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Slot",
                table: "Transactions",
                column: "idSlot",
                principalTable: "Slot",
                principalColumn: "id");
        }
    }
}
