using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddManualCheckNumber : Migration
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

            migrationBuilder.AddColumn<string>(
                name: "MANUAL_CHECK_NUMBER",
                table: "DISTRIBUTION",
                type: "NVARCHAR2(16)",
                maxLength: 16,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MANUAL_CHECK_NUMBER",
                table: "DISTRIBUTION");

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
