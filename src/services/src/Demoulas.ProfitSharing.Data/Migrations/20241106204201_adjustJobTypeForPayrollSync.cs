using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class adjustJobTypeForPayrollSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "JOBTYPE",
                keyColumn: "ID",
                keyValue: (byte)2);

            migrationBuilder.UpdateData(
                table: "JOBTYPE",
                keyColumn: "ID",
                keyValue: (byte)0,
                column: "NAME",
                value: "Employee Sync Full");

            migrationBuilder.UpdateData(
                table: "JOBTYPE",
                keyColumn: "ID",
                keyValue: (byte)1,
                column: "NAME",
                value: "Payroll Syn Full");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "JOBTYPE",
                keyColumn: "ID",
                keyValue: (byte)0,
                column: "NAME",
                value: "Full");

            migrationBuilder.UpdateData(
                table: "JOBTYPE",
                keyColumn: "ID",
                keyValue: (byte)1,
                column: "NAME",
                value: "Delta");

            migrationBuilder.InsertData(
                table: "JOBTYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[] { (byte)2, "Individual" });
        }
    }
}
