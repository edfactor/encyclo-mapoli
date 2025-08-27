using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addExecutiveAndOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)6,
                column: "NAME",
                value: "IT-DevOps");

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)7, "IT-Operations" },
                    { (byte)8, "Executive-Administrator" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)7);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)8);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)6,
                column: "NAME",
                value: "IT-Operations");
        }
    }
}
