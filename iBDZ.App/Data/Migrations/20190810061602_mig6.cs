using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace iBDZ.App.Data.Migrations
{
    public partial class mig6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Route",
                table: "Receipts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOfArrival",
                table: "Receipts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOfDeparture",
                table: "Receipts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TrainId",
                table: "Receipts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Route",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "TimeOfArrival",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "TimeOfDeparture",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "TrainId",
                table: "Receipts");
        }
    }
}
