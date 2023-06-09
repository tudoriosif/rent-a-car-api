using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentACarAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentingEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "RentingEvent",
                newName: "PricePerHour");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RentalStartDate",
                table: "RentingEvent",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RentalEndDate",
                table: "RentingEvent",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<double>(
                name: "TotalCost",
                table: "RentingEvent",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalRentingHours",
                table: "RentingEvent",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "RentingEvent");

            migrationBuilder.DropColumn(
                name: "TotalRentingHours",
                table: "RentingEvent");

            migrationBuilder.RenameColumn(
                name: "PricePerHour",
                table: "RentingEvent",
                newName: "Cost");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RentalStartDate",
                table: "RentingEvent",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RentalEndDate",
                table: "RentingEvent",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
