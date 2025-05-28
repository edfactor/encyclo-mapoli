using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class navigationStatusNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: (byte)3,
                column: "NAME",
                value: "On Hold");

            migrationBuilder.UpdateData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: (byte)4,
                column: "NAME",
                value: "Complete");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: (byte)3,
                column: "NAME",
                value: "Blocked");

            migrationBuilder.UpdateData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: (byte)4,
                column: "NAME",
                value: "Successful");
        }
    }
}
