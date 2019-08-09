using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace iBDZ.App.Data.Migrations
{
    public partial class mig5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_AspNetUsers_UserId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_UserId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "PriceLevs",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TimeOfPurchase",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Purchases");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceLevs",
                table: "Receipts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOfPurchase",
                table: "Receipts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Receipts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_UserId",
                table: "Receipts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_AspNetUsers_UserId",
                table: "Receipts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_AspNetUsers_UserId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_UserId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "PriceLevs",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "TimeOfPurchase",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Receipts");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceLevs",
                table: "Purchases",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOfPurchase",
                table: "Purchases",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Purchases",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_UserId",
                table: "Purchases",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_AspNetUsers_UserId",
                table: "Purchases",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
