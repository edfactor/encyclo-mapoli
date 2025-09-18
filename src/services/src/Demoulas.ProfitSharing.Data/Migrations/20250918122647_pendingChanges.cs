using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class pendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUS_STATUSID",
                table: "DISTRIBUTION");

            migrationBuilder.InsertData(
                table: "NAVIGATION",
                columns: new[] { "ID", "DISABLED", "ICON", "ORDER_NUMBER", "PARENT_ID", "STATUS_ID", "SUB_TITLE", "TITLE", "URL" },
                values: new object[] { (short)69, false, "", (byte)19, (short)14, (byte)1, "QPROF003-1", "Terminated Letters", "terminated-letters" });

            migrationBuilder.AddForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUSES_STATUSID",
                table: "DISTRIBUTION",
                column: "STATUS_ID",
                principalTable: "DISTRIBUTION_STATUS",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUSES_STATUSID",
                table: "DISTRIBUTION");

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)69);

            migrationBuilder.AddForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUS_STATUSID",
                table: "DISTRIBUTION",
                column: "STATUS_ID",
                principalTable: "DISTRIBUTION_STATUS",
                principalColumn: "ID");
        }
    }
}
