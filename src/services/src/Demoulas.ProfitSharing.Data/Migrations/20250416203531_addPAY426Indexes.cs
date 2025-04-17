using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addPAY426Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_MONTHTODATE",
                table: "PROFIT_DETAIL",
                columns: new[] { "PROFIT_YEAR", "MONTH_TO_DATE" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_PROFITCODEID",
                table: "PROFIT_DETAIL",
                columns: new[] { "PROFIT_YEAR", "PROFIT_CODE_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_SSN",
                table: "PROFIT_DETAIL",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_TERMINATIONDATE",
                table: "DEMOGRAPHIC",
                column: "TERMINATION_DATE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_MONTHTODATE",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_PROFITCODEID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropIndex(
                name: "IX_PROFIT_DETAIL_SSN",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_TERMINATIONDATE",
                table: "DEMOGRAPHIC");
        }
    }
}
