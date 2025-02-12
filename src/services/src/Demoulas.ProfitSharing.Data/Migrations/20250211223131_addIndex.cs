using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_SSN_PROFITYEAR_PROFITCODEID",
                table: "PROFIT_DETAIL",
                columns: new[] { "SSN", "PROFIT_YEAR", "PROFIT_CODE_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR",
                table: "PAY_PROFIT",
                column: "PROFIT_YEAR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PROFIT_DETAIL_SSN_PROFITYEAR_PROFITCODEID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR",
                table: "PAY_PROFIT");

            
        }
    }
}
