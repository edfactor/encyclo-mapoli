using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTaxCodeColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TAXCODEID",
                table: "PROFIT_SHARE_CHECK",
                newName: "TAX_CODE_ID");
         

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_SHARE_CHECK_SSN",
                table: "PROFIT_SHARE_CHECK",
                column: "SSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PROFIT_SHARE_CHECK_SSN",
                table: "PROFIT_SHARE_CHECK");

            migrationBuilder.RenameColumn(
                name: "TAX_CODE_ID",
                table: "PROFIT_SHARE_CHECK",
                newName: "TAXCODEID");
        }
    }
}
