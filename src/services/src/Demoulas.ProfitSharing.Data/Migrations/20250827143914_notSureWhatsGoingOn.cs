using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class notSureWhatsGoingOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)56,
                column: "TITLE",
                value: "IT DEVOPS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)56,
                column: "TITLE",
                value: "IT OPERATIONS");
        }
    }
}
