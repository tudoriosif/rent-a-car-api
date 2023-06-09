using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentACarAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentingEvents2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentingEvent_AspNetUsers_OwnerId",
                table: "RentingEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_RentingEvent_Cars_CarId",
                table: "RentingEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentingEvent",
                table: "RentingEvent");

            migrationBuilder.RenameTable(
                name: "RentingEvent",
                newName: "RentingEvents");

            migrationBuilder.RenameIndex(
                name: "IX_RentingEvent_OwnerId",
                table: "RentingEvents",
                newName: "IX_RentingEvents_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_RentingEvent_CarId",
                table: "RentingEvents",
                newName: "IX_RentingEvents_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentingEvents",
                table: "RentingEvents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RentingEvents_AspNetUsers_OwnerId",
                table: "RentingEvents",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RentingEvents_Cars_CarId",
                table: "RentingEvents",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentingEvents_AspNetUsers_OwnerId",
                table: "RentingEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_RentingEvents_Cars_CarId",
                table: "RentingEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentingEvents",
                table: "RentingEvents");

            migrationBuilder.RenameTable(
                name: "RentingEvents",
                newName: "RentingEvent");

            migrationBuilder.RenameIndex(
                name: "IX_RentingEvents_OwnerId",
                table: "RentingEvent",
                newName: "IX_RentingEvent_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_RentingEvents_CarId",
                table: "RentingEvent",
                newName: "IX_RentingEvent_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentingEvent",
                table: "RentingEvent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RentingEvent_AspNetUsers_OwnerId",
                table: "RentingEvent",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RentingEvent_Cars_CarId",
                table: "RentingEvent",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
