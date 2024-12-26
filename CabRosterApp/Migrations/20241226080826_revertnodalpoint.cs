using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabRosterApp.Migrations
{
    /// <inheritdoc />
    public partial class revertnodalpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CabBookings_NodalPointId",
                table: "CabBookings",
                column: "NodalPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_CabBookings_NodalPoints_NodalPointId",
                table: "CabBookings",
                column: "NodalPointId",
                principalTable: "NodalPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CabBookings_NodalPoints_NodalPointId",
                table: "CabBookings");

            migrationBuilder.DropIndex(
                name: "IX_CabBookings_NodalPointId",
                table: "CabBookings");
        }
    }
}
