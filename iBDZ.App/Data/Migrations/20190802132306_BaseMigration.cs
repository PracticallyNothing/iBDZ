using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace iBDZ.App.Data.Migrations
{
    public partial class BaseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    StartStation = table.Column<string>(nullable: true),
                    MiddleStation = table.Column<string>(nullable: true),
                    EndStation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trains",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    RouteId = table.Column<string>(nullable: true),
                    TimeOfDeparture = table.Column<DateTime>(nullable: false),
                    TimeOfArrival = table.Column<DateTime>(nullable: false),
                    Delay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trains_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainCars",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Class = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    TrainId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainCars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainCars_Trains_TrainId",
                        column: x => x.TrainId,
                        principalTable: "Trains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    SeatNumber = table.Column<int>(nullable: false),
                    Coupe = table.Column<int>(nullable: false),
                    CarId = table.Column<string>(nullable: true),
                    ReserverId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seats_TrainCars_CarId",
                        column: x => x.CarId,
                        principalTable: "TrainCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seats_AspNetUsers_ReserverId",
                        column: x => x.ReserverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seats_CarId",
                table: "Seats",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ReserverId",
                table: "Seats",
                column: "ReserverId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainCars_TrainId",
                table: "TrainCars",
                column: "TrainId");

            migrationBuilder.CreateIndex(
                name: "IX_Trains_RouteId",
                table: "Trains",
                column: "RouteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "TrainCars");

            migrationBuilder.DropTable(
                name: "Trains");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
