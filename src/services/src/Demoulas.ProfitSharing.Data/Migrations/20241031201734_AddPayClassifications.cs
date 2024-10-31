using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPayClassifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "DEPARTMENT",
                keyColumn: "ID",
                keyValue: (byte)6,
                column: "NAME",
                value: "Beer/Wine");

            migrationBuilder.InsertData(
                table: "PAY_CLASSIFICATION",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)4, "SPIRITS MANAGER" },
                    { (byte)5, "ASST SPIRITS MANAGER" },
                    { (byte)6, "SPIRITS CLERK - FT" },
                    { (byte)21, "ASST OFFICE MANAGER" },
                    { (byte)36, "MARKETS KITCHEN PT 16-17" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: (byte)5);

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: (byte)6);

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: (byte)21);

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: (byte)36);

            migrationBuilder.UpdateData(
                table: "DEPARTMENT",
                keyColumn: "ID",
                keyValue: (byte)6,
                column: "NAME",
                value: "Beer and Wine");
        }
    }
}
