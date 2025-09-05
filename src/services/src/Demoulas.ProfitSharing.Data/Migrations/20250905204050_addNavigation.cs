using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "NAVIGATION",
                columns: new[] { "ID", "DISABLED", "ICON", "ORDER_NUMBER", "PARENT_ID", "STATUS_ID", "SUB_TITLE", "TITLE", "URL" },
                values: new object[] { (short)68, false, "", (byte)18, (short)14, (byte)1, "", "Adhoc Beneficiaries Report", "adhoc-beneficiaries-report" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)68);
        }
    }
}
