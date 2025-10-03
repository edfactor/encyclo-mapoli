using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReportChecksumLookupIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "NAVIGATION",
                columns: new[] { "ID", "DISABLED", "ICON", "IS_NAVIGABLE", "ORDER_NUMBER", "PARENT_ID", "STATUS_ID", "SUB_TITLE", "TITLE", "URL" },
                values: new object[] { (short)79, false, "", null, (byte)1, (short)50, (byte)1, "", "ADJUSTMENTS", "adjustments" });

            migrationBuilder.CreateIndex(
                name: "IDX_REPORT_CHECKSUM_LOOKUP",
                table: "REPORT_CHECKSUM",
                columns: new[] { "PROFIT_YEAR", "REPORT_TYPE", "CREATED_AT_UTC" },
                descending: new[] { false, false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_REPORT_CHECKSUM_LOOKUP",
                table: "REPORT_CHECKSUM");

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)79);
        }
    }
}
