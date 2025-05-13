using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class healthId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__HEALTH_CHECK_STATUS_HISTORY",
                table: "_HEALTH_CHECK_STATUS_HISTORY");

            migrationBuilder.AlterColumn<string>(
                name: "KEY",
                table: "_HEALTH_CHECK_STATUS_HISTORY",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "_HEALTH_CHECK_STATUS_HISTORY",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0)
                .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK__HEALTH_CHECK_STATUS_HISTORY",
                table: "_HEALTH_CHECK_STATUS_HISTORY",
                column: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__HEALTH_CHECK_STATUS_HISTORY",
                table: "_HEALTH_CHECK_STATUS_HISTORY");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "_HEALTH_CHECK_STATUS_HISTORY");

            migrationBuilder.AlterColumn<string>(
                name: "KEY",
                table: "_HEALTH_CHECK_STATUS_HISTORY",
                type: "NVARCHAR2(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AddPrimaryKey(
                name: "PK__HEALTH_CHECK_STATUS_HISTORY",
                table: "_HEALTH_CHECK_STATUS_HISTORY",
                column: "KEY");
        }
    }
}
