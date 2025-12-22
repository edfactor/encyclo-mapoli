using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddForfeitClassActionAdjustmentCommentType : Migration
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
                values: new object[] { (byte)28, "Forfeiture adjustment for Class Action" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)28);

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
