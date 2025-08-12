using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addPay426Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR_DEMOGRAPHICID",
                table: "PAY_PROFIT",
                columns: new[] { "PROFIT_YEAR", "DEMOGRAPHIC_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTSTATUSID_TERMINATIONDATE",
                table: "DEMOGRAPHIC",
                columns: new[] { "EMPLOYMENT_STATUS_ID", "TERMINATION_DATE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR_DEMOGRAPHICID",
                table: "PAY_PROFIT");

            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTSTATUSID_TERMINATIONDATE",
                table: "DEMOGRAPHIC");
        }
    }
}
