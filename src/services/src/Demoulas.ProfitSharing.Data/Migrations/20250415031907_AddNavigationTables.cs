using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATION_NAVIGATION_STATUS_STATUS_ID",
                table: "NAVIGATION");

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATION_NAVIGATIONSTATUSES_STATUS_ID",
                table: "NAVIGATION",
                column: "STATUS_ID",
                principalTable: "NAVIGATION_STATUS",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATION_NAVIGATIONSTATUSES_STATUS_ID",
                table: "NAVIGATION");

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATION_NAVIGATION_STATUS_STATUS_ID",
                table: "NAVIGATION",
                column: "STATUS_ID",
                principalTable: "NAVIGATION_STATUS",
                principalColumn: "ID");
        }
    }
}
