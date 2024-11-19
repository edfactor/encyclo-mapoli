using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameBadgeNumberToEmployeeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BADGE_NUMBER",
                table: "DEMOGRAPHIC",
                newName: "EMPLOYEE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_DEMOGRAPHIC_BADGENUMBER",
                table: "DEMOGRAPHIC",
                newName: "IX_DEMOGRAPHIC_EMPLOYEEID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMPLOYEE_ID",
                table: "DEMOGRAPHIC",
                newName: "BADGE_NUMBER");

            migrationBuilder.RenameIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYEEID",
                table: "DEMOGRAPHIC",
                newName: "IX_DEMOGRAPHIC_BADGENUMBER");
        }
    }
}
