using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVestingNow100PctMissive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MISSIVES",
                columns: new[] { "ID", "MESSAGE" },
                values: new object[] { 2, "VEST IS NOW 100%, 65+/5 YRS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MISSIVES",
                keyColumn: "ID",
                keyValue: 2);
        }
    }
}
