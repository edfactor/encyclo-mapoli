using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReadOnlyToNavigationRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IS_READ_ONLY",
                table: "NAVIGATION_ROLE",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)1,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)2,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)3,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)4,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)5,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)6,
                column: "IS_READ_ONLY",
                value: true);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)7,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)8,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)9,
                column: "IS_READ_ONLY",
                value: true);

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)10,
                columns: new string[0],
                values: new object[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IS_READ_ONLY",
                table: "NAVIGATION_ROLE");
        }
    }
}
