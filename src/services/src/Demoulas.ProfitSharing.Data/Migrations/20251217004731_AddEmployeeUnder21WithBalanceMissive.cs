using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeUnder21WithBalanceMissive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.InsertData(
                table: "COMMENT_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[] { (byte)27, "Administrative - taking money from under 21" });

            migrationBuilder.InsertData(
                table: "MISSIVES",
                columns: new[] { "ID", "DESCRIPTION", "MESSAGE", "SEVERITY" },
                values: new object[] { 8, "Employee is currently under 21 and has a current or vested balance greater than zero.", "Employee under 21 w/ balance > 0", "Information" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)27);

            migrationBuilder.DeleteData(
                table: "MISSIVES",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);
        }
    }
}
