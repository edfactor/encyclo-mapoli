using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddYearEndPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTSTATUSID_HIREDATE_TERMINATIONDATE",
                table: "DEMOGRAPHIC",
                columns: new[] { "EMPLOYMENT_STATUS_ID", "HIRE_DATE", "TERMINATION_DATE" });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_HIREDATE",
                table: "DEMOGRAPHIC",
                column: "HIRE_DATE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTSTATUSID_HIREDATE_TERMINATIONDATE",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_HIREDATE",
                table: "DEMOGRAPHIC");
        }
    }
}
