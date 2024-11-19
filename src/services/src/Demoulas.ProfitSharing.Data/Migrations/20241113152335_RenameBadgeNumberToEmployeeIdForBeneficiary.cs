using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameBadgeNumberToEmployeeIdForBeneficiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BADGE_NUMBER",
                table: "BENEFICIARY",
                newName: "EMPLOYEE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_BENEFICIARY_BADGENUMBER",
                table: "BENEFICIARY",
                newName: "IX_BENEFICIARY_EMPLOYEEID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMPLOYEE_ID",
                table: "BENEFICIARY",
                newName: "BADGE_NUMBER");

            migrationBuilder.RenameIndex(
                name: "IX_BENEFICIARY_EMPLOYEEID",
                table: "BENEFICIARY",
                newName: "IX_BENEFICIARY_BADGENUMBER");

        }
    }
}
