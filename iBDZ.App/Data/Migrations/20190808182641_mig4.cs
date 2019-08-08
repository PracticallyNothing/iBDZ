using Microsoft.EntityFrameworkCore.Migrations;

namespace iBDZ.App.Data.Migrations
{
    public partial class mig4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptId",
                table: "Purchases",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ReceiptId",
                table: "Purchases",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Receipts_ReceiptId",
                table: "Purchases",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Receipts_ReceiptId",
                table: "Purchases");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_ReceiptId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "Purchases");
        }
    }
}
