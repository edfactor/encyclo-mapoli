using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDeltaSyncJobType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "JOBTYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[] { (byte)2, "Employee Sync Delta" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "JOBTYPE",
                keyColumn: "ID",
                keyValue: (byte)2);
        }
    }
}
