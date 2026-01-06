using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class customNavigationSettings2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NAVIGATION_CUSTOM_SETTING",
                table: "NAVIGATION_CUSTOM_SETTING");

            migrationBuilder.DeleteData(
                table: "NAVIGATION_CUSTOM_SETTING",
                keyColumn: "KEY",
                keyValue: "trackPageStatus");

            migrationBuilder.DeleteData(
                table: "NAVIGATION_CUSTOM_SETTING",
                keyColumn: "KEY",
                keyValue: "useFrozenYear");

            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "NAVIGATION_ID",
                table: "NAVIGATION_CUSTOM_SETTING",
                type: "NUMBER(5)",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NAVIGATION_CUSTOM_SETTING",
                table: "NAVIGATION_CUSTOM_SETTING",
                columns: new[] { "NAVIGATION_ID", "KEY" });

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATION_CUSTOM_SETTING_NAVIGATION_NAVIGATIONID",
                table: "NAVIGATION_CUSTOM_SETTING",
                column: "NAVIGATION_ID",
                principalTable: "NAVIGATION",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATION_CUSTOM_SETTING_NAVIGATION_NAVIGATIONID",
                table: "NAVIGATION_CUSTOM_SETTING");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NAVIGATION_CUSTOM_SETTING",
                table: "NAVIGATION_CUSTOM_SETTING");

            migrationBuilder.DropColumn(
                name: "NAVIGATION_ID",
                table: "NAVIGATION_CUSTOM_SETTING");

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NAVIGATION_CUSTOM_SETTING",
                table: "NAVIGATION_CUSTOM_SETTING",
                column: "KEY");

            migrationBuilder.InsertData(
                table: "NAVIGATION_CUSTOM_SETTING",
                columns: new[] { "KEY", "VALUE_JSON" },
                values: new object[,]
                {
                    { "trackPageStatus", "true" },
                    { "useFrozenYear", "true" }
                });
        }
    }
}
