using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addIndexs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CALDAR_RECORD_WEEKNO_PERIOD",
                table: "CALDAR_RECORD",
                columns: new[] { "ACC_WEEKN", "ACC_PERIOD" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CALDAR_RECORD_WEEKNO_PERIOD",
                table: "CALDAR_RECORD");
        }
    }
}
